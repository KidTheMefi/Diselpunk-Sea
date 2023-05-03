using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DefaultNamespace;
using ShellSelectorScripts;
using UnityEngine;

namespace ModulesScripts
{
    public class ArtilleryWithShellSelector : BaseArtillery
    {
        [SerializeField]
        private ShellSelector _shellSelector;

        private Shell _selectedShell;

        private void Awake()
        {
            _shellSelector.ShellTypeSelected += ShellSelectorOnShellTypeSelected;
            _selectedShell = _shell;
        }
        
        private void ShellSelectorOnShellTypeSelected(ShellType shellType)
        {
            _selectedShell = new Shell(_shell, shellType);
            SetActive(IsInOrder);
        }

        protected override Shell GetShell()
        {
            return _selectedShell;
        }

        protected override  UniTask ReloadAsync(CancellationToken token)
        {
            if (!_thisShip.ShellsHandler.HaveShells(_selectedShell.ShellType))
            {
                _shellSelector.SetDefaultShell();
                _selectedShell = _shell;
            }
            
            return base.ReloadAsync(token);
        }

        protected override UniTask Fire(CancellationToken token)
        {
            if (_thisShip.ShellsHandler.HaveShells(_selectedShell.ShellType))
            {
                _thisShip.ShellsHandler.SpentShell(_selectedShell.ShellType);
            }
            else
            {
                _shellSelector.SetDefaultShell();
                _selectedShell = _shell;
            }
            
            return base.Fire(token);
        }
    }
}