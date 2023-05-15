using System;
using DefaultNamespace;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SomeMenu
{
    public class SosDiseaseShipEvent : IMenuEvent
    {
    private BaseShip _playerShip;
    private MenuButtons _menuButtons;
    private event Action EndAction;

    private int _medicineSkillRequired;

    private bool _lootCargo;
    private bool _haveGasMask;
    private bool _knowAboutDisease;
    private bool _bodyChecked;
    private bool _captainDiaryReceived;
    private int _infectionChance;


    public SosDiseaseShipEvent(BaseShip playerShip, MenuButtons menuButtons, Action endAction)
    {
        _playerShip = playerShip;
        _menuButtons = menuButtons;
        EndAction = endAction;
        _medicineSkillRequired = Random.Range(2, 7);
    }

    public void BeginEvent()
    {
        string menuInfo = "You see a ship. It doesn't seem to be damaged at all, but you can't reach anyone on the radio.";
        _infectionChance = 0;

        _menuButtons.AddButton($"Sail closer", CloseInteraction);
        _menuButtons.AddButton("This is too suspicious. Leave", () =>
        {
            EndAction?.Invoke();
        });

        _menuButtons.ShowMenu(menuInfo);
    }

    private void CloseInteraction()
    {
        _menuButtons.RemoveButtons();
        string menuInfo = "As your vessel approached upon the strange ship, the scent of death filled the air, carried by the somber breeze.";

        _menuButtons.AddButton($"Send small team to that ship.", GoToShip);
        _menuButtons.AddButton("This is too dangerous. Leave", () =>
        {
            EndAction?.Invoke();
        });

        _menuButtons.ShowMenu(menuInfo);
    }

    private void GoToShip()
    {
        _menuButtons.RemoveButtons();

        _infectionChance++;
        string menuInfo = _haveGasMask ? "It's better now with gas mask, but you still don't wanna be this dead ship. " :
            "Dead smell at ship is terrifying. The team covers their noses with a handkerchief. ";


        menuInfo += _knowAboutDisease ? "You now about disease. You should probably leave." : null;

        if (!_bodyChecked)
        {
            menuInfo += $"\nYou see dead body nearby.";
            _menuButtons.AddButton($"Check the body", BodyExamination);
        }

        _menuButtons.AddButton($"Go to captains bridge to find some answers", CaptainBridge);
        _menuButtons.AddButton($"Go to lower deck, try to find useful things for your ship", LowerDeck);
        _menuButtons.AddButton("Leave before it's not too late", LeaveShip);

        _menuButtons.ShowMenu(menuInfo);
    }

    private void BodyExamination()
    {
        _infectionChance++;
        _bodyChecked = true;
        _menuButtons.RemoveButtons();
        string menuInfo = "You look at the body. Death is not violent.";

        _menuButtons.AddButtonInteractable($"Examine the body", () =>
        {
            if (_playerShip.MedicineValue() >= _medicineSkillRequired)
            {
                _knowAboutDisease = true;
                _infectionChance += 1;
                _menuButtons.AddTextToMenu("You realised that this is a disease that extremely contagious and deadly. The longer you are on the ship, the more dangerous it is. You need to leave ship as soon as possible.");
            }
            else
            {
                _infectionChance += 3;
                _menuButtons.AddTextToMenu("You can't figure out anything about the dead cause.");
            }
        }, true);

        _menuButtons.AddButton($"Leave", () =>
        {
            _bodyChecked = true;
            GoToShip();
        });
        _menuButtons.ShowMenu(menuInfo);
    }


    private void CaptainBridge()
    {
        _infectionChance++;
        _menuButtons.RemoveButtons();
        string menuInfo = "Captain is dead. Unlike other dead, that you saw at ship, cause of his death is obvious. He shoot himself";

        if (!_captainDiaryReceived)
        {
            _menuButtons.AddButtonInteractable($"Search for answers", () =>
            {
                _infectionChance++;
                _captainDiaryReceived = true;
                _knowAboutDisease = true;
                _menuButtons.AddTextToMenu("You find captain's diary. You learned about a rare disease that afflicted the crew of this ship. " +
                    "The captain realized the necessity of quarantine too late. The ship was doomed. " +
                    "At least they had the sense not to enter the port and spread the disease further.");
            }, true);
        }

        _menuButtons.AddButton($"Leave", GoToShip);
        _menuButtons.ShowMenu(menuInfo);
    }

    private void LowerDeck()
    {
        _infectionChance++;

        _menuButtons.RemoveButtons();
        string menuInfo = "Being on the lower decks is incredibly difficult.";

        menuInfo += _haveGasMask ? "Team is wearing gas masks, so now we can check ship's hold." : "The corpse stench is so strong that it seems the team is about to lose consciousness.";

        if (_haveGasMask)
        {
            if (!_lootCargo)
            {
                _menuButtons.AddButton("Go further", ShipsHold);
            }
        }
        else
        {
            _menuButtons.AddButton("Get gas mask from your ship", () =>
            {
                _infectionChance++;
                _haveGasMask = true;
                LowerDeck();
            });
        }
        _menuButtons.AddButton("Leave", GoToShip);

        _menuButtons.ShowMenu(menuInfo);
    }

    private void ShipsHold()
    {
        _menuButtons.RemoveButtons();
        string menuInfo = "There are lot of shells in the cargo. We can transfer them to our ship, but it will take some time and more crew.";

        _menuButtons.AddButton("Take shells", Loot);
        _menuButtons.AddButton("Leave", GoToShip);

        _menuButtons.ShowMenu(menuInfo);
    }

    private void Loot()
    {
        _menuButtons.RemoveButtons();
        _infectionChance += 6;
        _lootCargo = true;

        _playerShip.ShellsHandler.AddShell(ShellType.Shrapnel, 8);
        _playerShip.ShellsHandler.AddShell(ShellType.HighExplosive, 8);
        _playerShip.ShellsHandler.AddShell(ShellType.ArmorPiercing, 8);
        string menuInfo = "You have successfully taken all the cargo onto your ship";

        _menuButtons.AddButton("Leave", GoToShip);
        _menuButtons.ShowMenu(menuInfo);
    }

    private void LeaveShip()
    {
        _menuButtons.RemoveButtons();
        string menuInfo = "Your team come back. ";
        menuInfo += _knowAboutDisease ? "Since you know about the disease, you quarantine them." : null;

        if (_knowAboutDisease)
        {
            _menuButtons.AddButton($"Hope we did everything we can for our safety.", () =>
            {
                _infectionChance -= _playerShip.MedicineValue();
                IsDiseaseWillSpread();

            });
        }
        else
        {
            _menuButtons.AddButton($"Set new course", IsDiseaseWillSpread);
        }
        _menuButtons.ShowMenu(menuInfo);
    }

    private void DiseaseSpreadAtOurShip()
    {
        _menuButtons.RemoveButtons();
        string menuInfo = _knowAboutDisease ? "Despite the quarantine, the disease begins to spread throughout the ship." :
            "Some strange disease begins to spread throughout the ship.";


        _menuButtons.AddButton("Pray for us", () =>
        {
            int crewMaxDamage = _knowAboutDisease ? 2 : 4;
            if (_infectionChance > 10)
            {
                crewMaxDamage++;
            }
            int minDamage = 0;

            foreach (var place in _playerShip.Modules.modulesPlaces)
            {
                place.DamageCrew(Random.Range(minDamage, crewMaxDamage + 1));
            }

            EndEvent endEvent = new EndEvent(_menuButtons, EndAction);
            endEvent.BeginEvent("You lose some crew, but managed to stop disease", "Set new course");
        });
        _menuButtons.ShowMenu(menuInfo);
    }

    private void IsDiseaseWillSpread()
    {
        int chance = Random.Range(0, 10);
        bool disease = chance < _infectionChance;

        Debug.Log($"rand:{chance} / infection chance: {_infectionChance}");
        if (disease)
        {
            DiseaseSpreadAtOurShip();
        }
        else
        {
            EndAction?.Invoke();
        }
    }
    }
}