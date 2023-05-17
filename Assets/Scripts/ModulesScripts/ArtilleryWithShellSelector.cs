using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DefaultNamespace;
using InterfaceProviders;
using ShellSelectorScripts;
using ShipCharacteristics;
using UnityEngine;

namespace ModulesScripts
{
    public class ArtilleryWithShellSelector : BaseArtillery, IShellsHandlerRequired
    {
        [SerializeField]
        private ShellSelector _shellSelector;

        private Shell _selectedShell;
        private ShellsHandler _thisShipShellsHandler;

        public void SetShellsHandler(ShellsHandler shellsHandler)
        {
            _thisShipShellsHandler = shellsHandler;
        }
        
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
            if (!_thisShipShellsHandler.HaveShells(_selectedShell.ShellType))
            {
                _shellSelector.SetDefaultShell();
                _selectedShell = _shell;
            }
            
            return base.ReloadAsync(token);
        }

        protected override UniTask Fire(CancellationToken token)
        {
            if (_thisShipShellsHandler.HaveShells(_selectedShell.ShellType))
            {
                _thisShipShellsHandler.SpentShell(_selectedShell.ShellType);
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