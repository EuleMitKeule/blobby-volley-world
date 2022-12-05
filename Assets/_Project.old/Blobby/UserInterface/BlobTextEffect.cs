using System.Threading;
using System.Threading.Tasks;
using Blobby.Models;
using TMPro;
using UnityEngine;

namespace Blobby.UserInterface
{
    public class BlobTextEffect
    {
        GameObject _blobTextObj;
        CancellationTokenSource _tokenSource;

        public BlobTextEffect(GameObject playerObj, PlayerData playerData, string text)
        {
            if (text.Length > 20) 
            {
                text = text.Substring(0, 17);
                text += "...";
            }
            _tokenSource = new CancellationTokenSource();

            Task.Run(WaitToDisable);

            var canvasBlobText = playerObj.transform.Find("canvas_blob_text");
            _blobTextObj = canvasBlobText.transform.Find($"label_blob_text_{(playerData.Side == Side.Left ? "right" : "left")}").gameObject;

            var label = _blobTextObj.GetComponent<TextMeshProUGUI>();
            label.text = text;
            label.color = playerData.Color;

            var canvasGroup = _blobTextObj.GetComponent<CanvasGroup>();
            canvasGroup.alpha = 1;
        }

        async Task WaitToDisable()
        {
            try
            {
                await Task.Delay(1000, _tokenSource.Token);
            }
            catch (TaskCanceledException) { }

            var canvasGroup = _blobTextObj.GetComponent<CanvasGroup>();
            canvasGroup.alpha = 0;
        }
    }
}
