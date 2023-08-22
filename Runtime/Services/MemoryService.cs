// // ----------------------------------------------------------------------------
// // <copyright file="PersistentDataManager.cs" company="Kinetix SAS">
// // Kinetix Unity SDK - Copyright (C) 2022 Kinetix SAS
// // </copyright>
// // ----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Kinetix.Internal.Cache;
using Kinetix.Utils;
using Newtonsoft.Json;
using UnityEngine;

namespace Kinetix.Internal
{
    public class MemoryService: IKinetixService
    {
        private float MaxStorageSizeInMB = 50f;
        private float MaxRAMSizeinMB     = 50f;

        private const float MinStorageSizeInMB = 50f;
        private const float MinRamSizeInMB     = 50f;

        private double currentRAM;
        private double currentStorage;

        // Used by other services or managers to ask this service not to delete a file
        private List<string> inUseUUIDs;
        // List of files to delete when they can't be deleted at the moment
        private List<string> toDeleteFilesUUIDs;

        internal KinetixCacheManifest CacheManifest => cacheManifest;
        private KinetixCacheManifest cacheManifest;

        private const string c_CacheManifestKey = "KinetixSmartCache";

        public MemoryService(KinetixCoreConfiguration _Configuration)
        {
            MaxStorageSizeInMB = MinStorageSizeInMB;
            MaxRAMSizeinMB     = MinRamSizeInMB;

            inUseUUIDs = new List<string>();
            toDeleteFilesUUIDs = new List<string>();

            currentStorage = ByteConverter.ConvertBytesToMegabytes(FileUtils.GetSizeOfAnimationsCache());
            currentRAM     = 0.0f;

            LoadManifest(_Configuration.CachedEmotesNb);
        }

        #region RAM

        public void CheckRAM(string _ExcludeUUID = null, KinetixAvatar _ExcludeAvatar = null)
        {
            if (HasRAMExceedMemoryLimit())
                CleanRAM(_ExcludeUUID, _ExcludeAvatar);
        }
        
        public bool HasRAMExceedMemoryLimit()
        {
            return GetRAMSize() > MaxRAMSizeinMB;
        }

        public double GetRAMSize()
        {
            return ByteConverter.ConvertBytesToMegabytes((long) currentRAM);
        }

        public void AddRamAllocation(float _RamAllocationInBytes)
        {
            currentRAM += _RamAllocationInBytes;
            KinetixDebug.Log("RAM Size : " + GetRAMSize().ToString("F1") + "MB");
        }

        public void RemoveRamAllocation(float _RamAllocationInBytes)
        {
            currentRAM -= _RamAllocationInBytes;
            KinetixDebug.Log("RAM Size : " + GetRAMSize().ToString("F1") + "MB");
        }

        private void CleanRAM(string _ExcludeUUID = null, KinetixAvatar _ExcludeAvatar = null)
        {
            CleanRAMNonLocalPlayerAnimation(_ExcludeUUID, _ExcludeAvatar);
        }
        
        private void CleanRAMNonLocalPlayerAnimation(string _ExcludeUUID, KinetixAvatar _ExcludeAvatar)
        {
            KinetixEmote[] emotes = KinetixCoreBehaviour.ServiceLocator.Get<EmotesService>().GetAllEmotes();
            for (int i = 0; i < emotes.Length; i++)
            {
                if (!HasRAMExceedMemoryLimit())
                    return;

                // Clear emote clip for all avatars except exclude avatar and local player avatar if UUID match
                // Otherwise clean all avatars except local player avatar
                ForceClearEmote(emotes[i], emotes[i].Ids.UUID == _ExcludeUUID ? new[] { KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().LocalPlayer.KAvatar, _ExcludeAvatar } : new[] { KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().LocalPlayer.KAvatar });
            }
        }
        
        private void CleanRAMLocalPlayerAnimation(string _ExcludeUUID, KinetixAvatar _ExcludeAvatar)
        {
            if (!HasRAMExceedMemoryLimit())
                    return;
        
            KinetixEmote[] emotes = KinetixCoreBehaviour.ServiceLocator.Get<EmotesService>().GetAllEmotes();

            for (int i = 0; i < emotes.Length; i++)
            {
                ForceClearEmote(emotes[i], emotes[i].Ids.UUID == _ExcludeUUID ? new[] {  _ExcludeAvatar } : new KinetixAvatar[] { } );
            }
        }

        private void ForceClearEmote(KinetixEmote _kinetixEmote, KinetixAvatar[] _AvoidAvatars = null)
        {
            _kinetixEmote.ClearAllAvatars(_AvoidAvatars);
        }
        
        public void DeleteFileInRaM(UnityEngine.Object _obj)
        {
            UnityEngine.Object.Destroy(_obj);
        }

        #endregion

        #region STORAGE MANAGEMENT

        public void CheckStorage()
        {
            if (HasStorageExceedMemoryLimit())
                CleanStorage();
        }
        
        public bool FileExists(string _Path)
        {
            return File.Exists(_Path);
        }
        
        public bool HasStorageExceedMemoryLimit()
        {
            return GetStorageSize() > MaxStorageSizeInMB;
        }
        
        private double GetStorageSize()
        {
            return ByteConverter.ConvertBytesToMegabytes(FileUtils.GetSizeOfAnimationsCache());
        }
        
        public void AddStorageAllocation(string _Path, bool _Log = true)
        {
            if (File.Exists(_Path))
            {
                currentStorage += new FileInfo(_Path).Length;
                if (_Log)
                    KinetixDebug.Log("Storage Size : " + GetStorageSize().ToString("F1") + "MB");
            }
        }

        public void RemoveStorageAllocation(string _Path, bool _Log = true)
        {
            if (File.Exists(_Path))
            {
                currentStorage -= new FileInfo(_Path).Length;
            }
        }
        
        private void CleanStorage()
        {
            if (!HasStorageExceedMemoryLimit())
                return;

            CleanStorageNonLocalPlayerAnimation();
            CleanStorageLocalPlayerAnimation();
        }
        
        private void CleanStorageObsoleteAnimations()
        {
            // Clear first non use files
            if (!Directory.Exists(PathConstants.CacheAnimationsPath))
                return;
            
            DirectoryInfo cacheDirInfo = new DirectoryInfo(PathConstants.CacheAnimationsPath);
            FileInfo[] files = cacheDirInfo.GetFiles();
            
            foreach (FileInfo file in files)
            {
                string animUUIDWithoutExtension = Path.GetFileNameWithoutExtension(file.Name);
                DeleteFileInStorage(Path.Combine(animUUIDWithoutExtension), false);
            }
        }

        private void CleanStorageNonLocalPlayerAnimation()
        {
            KinetixEmote[] emotes = KinetixCoreBehaviour.ServiceLocator.Get<EmotesService>().GetAllEmotes();
            for (int i = 0; i < emotes.Length; i++)
            {
                if (!HasStorageExceedMemoryLimit())
                    return;

                if (inUseUUIDs.Contains(emotes[i].Ids.UUID))
                    continue;

                if (!KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().LocalPlayer.IsEmoteUsedByPlayer(emotes[i].Ids))
                {
                    DeleteFileInStorage(emotes[i].Ids.UUID);
                }
            }
        }
        
        private void CleanStorageLocalPlayerAnimation()
        {
            KinetixEmote[] emotes = KinetixCoreBehaviour.ServiceLocator.Get<EmotesService>().GetAllEmotes();
            for (int i = 0; i < emotes.Length; i++)
            {
                if (!HasStorageExceedMemoryLimit())
                    return;
                
                if (inUseUUIDs.Contains(emotes[i].Ids.UUID))
                    continue;

                if (KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().LocalPlayer.IsEmoteUsedByPlayer(emotes[i].Ids))
                {
                    DeleteFileInStorage(emotes[i].Ids.UUID);
                }
            }
        }

        public void TagFileAsBeingInUse(string _UUID)
        {
            inUseUUIDs.Add(_UUID);
        }

        public void OnFileStopBeingUsed(string _UUID)
        {
            inUseUUIDs.Remove(_UUID);
            EmptyDeletionList();
        }
        
        public void DeleteFileInStorage(string _UUID, bool _Log = true)
        {
            string path = Path.Combine(PathConstants.CacheAnimationsPath, (_UUID + ".glb"));

            if (inUseUUIDs.Contains(_UUID) && !toDeleteFilesUUIDs.Contains(_UUID))
            {
                toDeleteFilesUUIDs.Add(_UUID);
                return;
            }

            if (File.Exists(path))
            {
                RemoveStorageAllocation(path, _Log);
                File.Delete(path);
                toDeleteFilesUUIDs.Remove(_UUID);
                
                if (_Log)
                    KinetixDebug.Log("Storage Size : " + (GetStorageSize().ToString("F1") + "MB"));
            }
        }

        public void EmptyDeletionList()
        {
            foreach (string UUID in toDeleteFilesUUIDs)
                DeleteFileInStorage(UUID);
        }
        
        #endregion
    
        #region SMART CACHE

        public void OnAnimationLoadedOnPlayer(AnimationIds _Ids)
        {
            // Check if cache is full and does not contain the anim already
            if (!cacheManifest.Contains(_Ids))
            {
                if (cacheManifest.IsFull())
                {
                    cacheManifest.Reorder();
                    cacheManifest.RemoveLast();
                }

                cacheManifest.Add(_Ids);
            }

            // Animation added or not, we reorder to have a clean manifest
            cacheManifest.Reorder();
  
            SaveManifest();
        }

        public void OnAnimationUnloadedOnPlayer()
        {
            cacheManifest.Reorder();

            SaveManifest();
        }

        public void LoadManifest(int _AnimationNb)
        {            
            string serializedManifest = PlayerPrefs.GetString(c_CacheManifestKey, "");

            // If the manifest could not be fetched, create one
            if (serializedManifest == string.Empty)
            {
                CreateManifest(_AnimationNb);
                return;
            }

            cacheManifest = JsonConvert.DeserializeObject<KinetixCacheManifest>(serializedManifest);
            
            cacheManifest.TargetAnimationNb = _AnimationNb;
            cacheManifest.OnAnimationRemoved += OnAnimationRemovedFromCache;
            cacheManifest.CleanUntilTargetNumber();
        }

        public void CleanCache()
        {
            if (cacheManifest == null)
                return;

            foreach (KinetixCachedAnimation animation in cacheManifest.Animations)
            {
                DeleteFileInStorage(animation.Ids.UUID);
            }

            PlayerPrefs.DeleteKey(c_CacheManifestKey);
        }

        private void CreateManifest(int _AnimatioNb)
        {
            cacheManifest = new KinetixCacheManifest(_AnimatioNb);

            // Read dir and add to now if there are some
            ReadCachedAnimAndAdd();

            SaveManifest();
        }


        private void ReadCachedAnimAndAdd()
        {
            if (!Directory.Exists(PathConstants.CacheAnimationsPath))
                return;
            
            DirectoryInfo di = new DirectoryInfo(PathConstants.CacheAnimationsPath);
            FileInfo[] files = di.GetFiles();

            foreach (FileInfo file in files)
            {
                string animUUIDWithoutExtension = Path.GetFileNameWithoutExtension(file.Name);

                cacheManifest.Add(new AnimationIds(animUUIDWithoutExtension));
            }
        }

        private void SaveManifest()
        {
            PlayerPrefs.SetString(c_CacheManifestKey, JsonConvert.SerializeObject(cacheManifest));
            PlayerPrefs.Save();

            EmptyDeletionList();
        }

        private void OnAnimationRemovedFromCache(AnimationIds _Ids)
        {
            DeleteFileInStorage(_Ids.UUID);
        }

        #endregion
    }
}
