using System;
using UnityEngine;

namespace Generator.Ring.Helper
{
    [RequireComponent(typeof(Generator))]
    public class Component: MonoBehaviour
    {
        protected Generator Generator;

        private void Awake()
        {
            Generator = GetComponent<Generator>();
        }
    }
}
