using MelonLoader;
using UnityEngine;

namespace StarRailMod
{
    public static class ModBuildInfo
    {
        public const string Name = "HSR-Mod";
        public const string Author = "Taiga74164";
        public const string Version = "1.0.0";
        public const string DownloadLink = null;
        public const string Company = null;
        public const string GameDeveloper = "HoYoverse";
        public const string Game = "Honkai: Star Rail";
    }

    public class Main : MelonMod
    {
        private GameObject _activeCharacter;
        private Camera _mainCamera;

        private bool _noClip = false;
        private bool _fastDialog = false;
        private bool _didSpeed = false;
        private bool _hideUID = false;
        private bool _hideUI = false;

        public override void OnApplicationStart()
        {
            MelonLogger.Msg("Honkai: Star Rail Mod");
            MelonLogger.Msg("BackQuote = NoClip");
            MelonLogger.Msg("Alpha6 = FastDialog");
            MelonLogger.Msg("Alpha7 = HideUID");
            MelonLogger.Msg("Alpha8 = HideUI");
        }

        public override void OnUpdate()
        {
            var playerRoot = GameObject.Find("EntityRoot/PlayerRoot/");
            if (playerRoot == null)
                return;

            var camObj = GameObject.Find("MainCamera");
            if (camObj == null)
                return;

            if (_mainCamera == null || !camObj.activeInHierarchy)
                _mainCamera = camObj.GetComponent<Camera>();
            
            if (_activeCharacter == null || !_activeCharacter.activeInHierarchy)
                FindActiveCharacter(playerRoot);

            if (Input.GetKeyDown(KeyCode.BackQuote))
                _noClip = !_noClip;
            if (Input.GetKeyDown(KeyCode.Alpha6))
                _fastDialog = !_fastDialog;
            if (Input.GetKeyDown(KeyCode.Alpha7))
                _hideUID = !_hideUID;
            if (Input.GetKeyDown(KeyCode.Alpha8))
                _hideUI = !_hideUI;


            // Features
            NoClip();
            //FastDialog();
            //HideUID();
            //HideUI();
            //get_DitherAlphaVisible

        }

        #region HELPERS

        private void FindActiveCharacter(GameObject root)
        {
            if (root == null)
                return;

            if (root.transform.childCount == 0)
                return;

            for (int i = 0; i < root.transform.childCount; i++)
            {
                var child = root.transform.GetChild(i);
                if (!child.gameObject.activeInHierarchy)
                    continue;

                MelonLogger.Msg($"Found active character: {child.gameObject.name}");
                _activeCharacter = child.gameObject;
            }
        }

        #endregion

        #region Features

        private void NoClip()
        {
            if (!_noClip)
                return;

            Vector3 dir = new Vector3();
            var relativePos = _mainCamera.transform;

            // Horizontal
            if (Input.GetKey(KeyCode.W))
                dir = dir + relativePos.forward;
            if (Input.GetKey(KeyCode.S))
                dir =  dir + (relativePos.forward * -1);
            if (Input.GetKey(KeyCode.D))
                dir = dir + relativePos.right;
            if (Input.GetKey(KeyCode.A))
                dir = dir + (relativePos.right * -1);

            // Vertical
            if (Input.GetKey(KeyCode.Space))
                dir = dir + _mainCamera.transform.up;
            if (Input.GetKey(KeyCode.LeftControl))
                dir = dir + (_mainCamera.transform.up * -1);

            var prevPos = _activeCharacter.transform.localPosition;
            if (prevPos.Equals(Vector3.zero))
                return;

            var newPos = prevPos + dir * 20f * Time.deltaTime;
            _activeCharacter.transform.localPosition = newPos;
        }

        private void FastDialog()
        {
            var talkDialogObj = GameObject.Find("GameObjectPoolRoot/TalkDialog(Clone)");
            if (talkDialogObj == null) 
                return;
            
            if (_fastDialog)
            {
                if (talkDialogObj.active)
                    Time.timeScale = 5f;
                else
                    Time.timeScale = 1f;

                _didSpeed = false;
            }
            else
            {
                if (!_didSpeed)
                {
                    Time.timeScale = 1f;
                    _didSpeed = true;
                }
            }

        }

        private void HideUID()
        {
            var uidObj = GameObject.Find("UIRoot/AboveDialog/BetaHintDialog(Clone)");

            if (_hideUID)
                uidObj.SetActive(false);
            else
                uidObj.SetActive(true);
        }

        private void HideUI()
        {
            var uiCamera = GameObject.Find("UICamera");

            if (_hideUI)
                uiCamera.SetActive(false);
            else
                uiCamera.SetActive(true);
        }

        #endregion
    }
}