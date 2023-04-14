// // ----------------------------------------------------------------------------
// // <copyright file="AnimationIds.cs" company="Kinetix SAS">
// // Kinetix Unity SDK - Copyright (C) 2022 Kinetix SAS
// // </copyright>
// // ----------------------------------------------------------------------------

using System;
using UnityEngine;

[Serializable]
public class AnimationIds
{
    
    // /!\ Not readonly for serialisation
    
    public string UUID;

    public AnimationIds(string _UUID)
    {
        UUID = _UUID;
    }

    public string Serialize()
    {
        return JsonUtility.ToJson(this);
    }

    public AnimationIds Deserialize(string _JSON)
    {
        return JsonUtility.FromJson<AnimationIds>(_JSON);
    }

    #region Equals
    public override bool Equals(object obj)
    {
        if ((obj == null) || GetType() != obj.GetType())
            return false;
        AnimationIds animationIds = (AnimationIds)obj;
        if (!string.IsNullOrEmpty(UUID))
            return UUID == animationIds.UUID;
        return (UUID == animationIds.UUID);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hashCode = (UUID != null ? UUID.GetHashCode() : 0);
            if (!string.IsNullOrEmpty(UUID))
                return hashCode;
            return hashCode;
        }
    }
    #endregion
    
    public override string ToString()
    {
        return "\n" + "UUID : " + UUID;
    }
    
}
