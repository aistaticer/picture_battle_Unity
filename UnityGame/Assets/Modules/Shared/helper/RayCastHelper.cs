using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace picture_game_view.Assets.Modules.Shared.helper
{
    public class RayCastHelper : MonoBehaviour
    {
        /// <summary>
        /// クリックしてオブジェクトを取得できた場合、RayCastで返す
        /// 取得できなかった場合はnullを返す
        /// </summary>
        /// <returns>クリックしたオブジェクト</returns>
        public static RaycastHit? GetRaycastByClick()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                // RaycastHit 構造体を用意して結果を受け取る
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    return hit;
                }
            }

            return null;
        }
    }
}