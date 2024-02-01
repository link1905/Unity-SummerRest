﻿using System;
using UnityEngine;

namespace SummerRest.Editor.Models
{
    [Serializable]
    public class SmartString : ISerializationCallbackReceiver
    {
        [SerializeField] private string value;
        [SerializeField] private string key;

        // Trigger editor redraw 
        public string Value => value;

        public string Key
        {
            get;
            set;
        }

        public SmartString(string key)
        {
            this.Key = key;
        }

        public void OnBeforeSerialize()
        {
            key = Key;
        }
        public void OnAfterDeserialize()
        {
        }
    }
}