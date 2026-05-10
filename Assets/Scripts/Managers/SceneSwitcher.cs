using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Managers
{
    public class SceneSwitcher : MonoBehaviour
    {
        public void OnGameStart()
        {
            SceneManager.LoadScene("Game");
        }
    }
}
