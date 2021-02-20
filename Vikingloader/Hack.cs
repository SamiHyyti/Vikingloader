using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Vikingloader
{
    public class Hack : MonoBehaviour
    {
        private bool ShowMenu = false;
        private bool toggleSpeedhack = false;
        private float pSpeed;
        Player localPlayer;
        private string text1,text2,text3,text4;
        private bool charEsp = false, berryEsp = false, oreEsp = false;
        private Rect wRect = new Rect(0, 0, 200, 300);
        public void Start()
        {
            pSpeed = 4f;
            text1 = "Speedhack <color=red>OFF</color>";
            text2 = "ESP <color=red>OFF</color>";
            text3 = "berryESP <color=red>OFF</color>";
            text4 = "oreESP <color=red>OFF</color>";
        }
        public void Update()
        {
            localPlayer = Player.m_localPlayer;
            if (localPlayer != null)
            {
                if (!toggleSpeedhack)
                {
                    localPlayer.m_speed = pSpeed;
                    text1 = "Speedhack <color=red>OFF</color>";
                }
                else
                {
                    localPlayer.m_speed = pSpeed * 3;
                    text1 = "Speedhack <color=green>ON</color>";
                }

                if (!charEsp)
                {
                    text2 = "ESP <color=red>OFF</color>";
                }
                else
                {
                    text2 = "ESP <color=green>ON</color>";
                }
                if (!berryEsp)
                {
                    text3 = "berryESP <color=red>OFF</color>";
                }
                else
                {
                    text3 = "berryESP <color=green>ON</color>";
                }
                if (!oreEsp)
                {
                    text4 = "oreESP <color=red>OFF</color>";
                }
                else
                {
                    text4 = "oreESP <color=green>ON</color>";
                }
            }
            if (Input.GetKeyDown(KeyCode.Home))
            {
                wRect = new Rect(0, 0, 200, 300);
                charEsp = false;
                berryEsp = false;
                oreEsp = false;
                ShowMenu = false;
            }
            if (Input.GetKeyDown(KeyCode.Insert))
            {
                ShowMenu = !ShowMenu;
                if (ShowMenu)
                {
                    var ismouseCapture = typeof(GameCamera).GetField("m_mouseCapture", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    ismouseCapture.SetValue(GameCamera.instance, false);
                    Console.instance.m_chatWindow.gameObject.SetActive(true);
                }
                else
                {
                    var ismouseCapture = typeof(GameCamera).GetField("m_mouseCapture", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    ismouseCapture.SetValue(GameCamera.instance, true);
                    Console.instance.m_chatWindow.gameObject.SetActive(false);
                }
            }
            if (Input.GetKeyDown(KeyCode.Delete))
            {
                Loader.Unload();
            }
        }
        public void OnGUI()
        {
            if (charEsp) DrawEsp();
            if (berryEsp) DrawBerryEsp();
            if (oreEsp) DrawRockEsp();
            if (!ShowMenu) return;
            wRect = GUI.Window(0, wRect, new GUI.WindowFunction(DoMyWindow), "", new GUIStyle());
        }
        public void DoMyWindow(int windowID)
        {
            GUIStyle style = new GUIStyle(GUI.skin.box);
            style.normal.background = MakeTex(2, 2, new Color(0f, 0f, 0f, 0.8f));
            
            UIHelper.Begin("<b>VikingHack</b>", 0, 0, 200, 300, 15, 30, 5, style);
            if (UIHelper.Button("Heal"))
            {
                localPlayer.Heal(100);
            }
            UIHelper.Label("Current speed: " + localPlayer.m_speed.ToString());
            if (UIHelper.Button(text1))
            {
                toggleSpeedhack = !toggleSpeedhack;
            }
            if (UIHelper.Button(text2))
            {
                charEsp = !charEsp;
            }
            if (UIHelper.Button(text3))
            {
                berryEsp = !berryEsp;
            }
            if (UIHelper.Button(text4))
            {
                oreEsp = !oreEsp;
            }
            //var ismouseCapture = typeof(GameCamera).GetField("m_mouseCapture", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            //UIHelper.Label("Enabled: "+ismouseCapture.GetValue(GameCamera.instance).ToString());
            GUI.DragWindow();
        }
        private Texture2D MakeTex(int width, int height, Color col)
        {
            Color[] pix = new Color[width * height];
            for (int i = 0; i < pix.Length; ++i)
            {
                pix[i] = col;
            }
            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();
            return result;
        }
        List<Character> characterList;
        Pickable[] pickableList;
        MineRock5[] ore5List;
        MineRock[] oreList;
        float espCache;
        private void DrawEsp()
        { 
            if (Time.time >= espCache)
            {
                espCache = Time.time + .5f;//.5 seconds till next objectlist scan
                //characterList = FindObjectsOfType<Character>();
                characterList = Character.GetAllCharacters();
            }
            GUIStyle espColor = new GUIStyle();
            espColor.normal.textColor = Color.yellow;
            if (charEsp)
            {
                foreach (Character character in characterList)
                {
                    if(character != Player.m_localPlayer)
                    {
                        Vector3 pos = character.transform.position;
                        Vector3 posScreen = Camera.main.WorldToScreenPoint(pos);
                        float dist = Vector3.Distance(pos, localPlayer.transform.position);
                        if (posScreen.z > 0 & posScreen.y < Screen.width - 2)
                        {
                            posScreen.y = Screen.height - (posScreen.y + 1f);
                            GUI.Label(new Rect(posScreen.x, posScreen.y, 200, 40), character.GetHoverName() + string.Format("(Lv:{0:0})", (object)character.GetLevel()) + string.Format(" [{0:0}]", (object)dist), espColor);
                        }
                    }
                }
            }
        }
        float berryCache;
        private void DrawBerryEsp()
        {
            if (Time.time >= berryCache)
            {
                berryCache = Time.time + 2f;//3 seconds till next objectlist scan
                //characterList = FindObjectsOfType<Character>();
                pickableList = FindObjectsOfType<Pickable>();
            }
            GUIStyle espColor = new GUIStyle();
            espColor.normal.textColor = Color.red;
            if (berryEsp)
            {
                foreach (Pickable pickable in pickableList)
                {
                    if (pickable.GetHoverText().Length > 0 && pickable.GetHoverName().Contains("erries"))
                    {
                        Vector3 pos = pickable.transform.position;
                        Vector3 posScreen = Camera.main.WorldToScreenPoint(pos);
                        float dist = Vector3.Distance(pos, localPlayer.transform.position);
                        if (posScreen.z > 0 & posScreen.y < Screen.width - 2)
                        {
                            posScreen.y = Screen.height - (posScreen.y + 1f);
                            GUI.Label(new Rect(posScreen.x, posScreen.y, 200, 40), pickable.GetHoverName() + string.Format(" [{0:0}]", (object)dist), espColor);
                        }
                    }
                    
                }
            }
        }
        float oreCache;
        private void DrawRockEsp()
        {
            if (Time.time >= oreCache)
            {
                oreCache = Time.time + 2f;//.5 seconds till next objectlist scan
                //characterList = FindObjectsOfType<Character>();
                oreList = FindObjectsOfType<MineRock>();
                ore5List = FindObjectsOfType<MineRock5>();
            }
            GUIStyle espColor = new GUIStyle();
            espColor.normal.textColor = Color.blue;
            if (oreEsp)
            {
                foreach (MineRock5 ore in ore5List)
                {
                    if(ore.GetHoverName().Length > 0)
                    {
                        Vector3 pos = ore.transform.position;
                        Vector3 posScreen = Camera.main.WorldToScreenPoint(pos);
                        float dist = Vector3.Distance(pos, localPlayer.transform.position);
                        if (posScreen.z > 0 & posScreen.y < Screen.width - 2)
                        {
                            posScreen.y = Screen.height - (posScreen.y + 1f);
                            GUI.Label(new Rect(posScreen.x, posScreen.y, 200, 40), ore.GetHoverName() + string.Format(" [{0:0}]", (object)dist), espColor);
                        }
                    }
                }
                foreach (MineRock ore in oreList)
                {
                    if (ore.GetHoverName().Length > 0)
                    {
                        Vector3 pos = ore.transform.position;
                        Vector3 posScreen = Camera.main.WorldToScreenPoint(pos);
                        float dist = Vector3.Distance(pos, localPlayer.transform.position);
                        if (posScreen.z > 0 & posScreen.y < Screen.width - 2)
                        {
                            posScreen.y = Screen.height - (posScreen.y + 1f);
                            GUI.Label(new Rect(posScreen.x, posScreen.y, 200, 40), ore.GetHoverName() + string.Format(" [{0:0}]", (object)dist), espColor);
                        }
                    }
                }
            }
        }
    }
}
