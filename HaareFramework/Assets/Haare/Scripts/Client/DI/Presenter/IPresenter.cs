using System;
using Cysharp.Threading.Tasks;
using Haare.Client.UI;
using R3;
using Unity.VisualScripting;
using UnityEngine;
using VContainer.Unity;

namespace Haare.Client.Core.DI
{
    public interface IPresenter : IPostInitializable, IDisposable
    {
        public CompositeDisposable disposables { get; }
        
    }
}