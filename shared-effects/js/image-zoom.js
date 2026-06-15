/* =============================================================
 * image-zoom.js — 圖片縮放特效輔助（共用類別，可選用）
 * -------------------------------------------------------------
 * 來源：原專案 inside.css / video.css 的 :hover { transform: scale(1.25) }
 * 縮放本身是純 CSS（見 effects.css 的 .fx-zoom）；
 * 本檔只是「方便地把 .fx-zoom 套到一批元素上」並可指定倍率，
 * 讓你不必逐一改 HTML。不需要時可完全不引入本檔。
 *
 * ---------------------- 使用方式 -----------------------------
 * 方式 A — 只用 CSS（推薦，最單純）：
 *   <img class="fx-zoom" src="/images/p.jpg">
 *   想改倍率：<img class="fx-zoom" style="--fx-zoom-scale:1.4" src="...">
 *
 * 方式 B — 用 JS 一次套用到多個元素：
 *   <img class="product-pic" src="...">
 *   <script src="/js/image-zoom.js"></script>
 *   <script>
 *     FxImageZoom.apply('.product-pic', 1.3);   // 對所有 .product-pic 套用，倍率 1.3
 *   </script>
 *
 * .NET MVC Razor：
 *   <script src="~/js/image-zoom.js"></script>
 *   <script> FxImageZoom.apply('.product-pic'); </script>
 *
 * 注意：請務必同時引入 effects.css，特效樣式定義在那裡。
 * ============================================================= */
(function (global) {
    'use strict';

    var FxImageZoom = {
        /**
         * 將縮放特效套用到符合選擇器的所有元素。
         * @param {string} selector CSS 選擇器，例如 '.product-pic'
         * @param {number} [scale]  放大倍率，預設 1.25
         */
        apply: function (selector, scale) {
            var nodes = document.querySelectorAll(selector);
            for (var i = 0; i < nodes.length; i++) {
                nodes[i].classList.add('fx-zoom');
                if (scale) {
                    nodes[i].style.setProperty('--fx-zoom-scale', scale);
                }
            }
            return nodes.length;
        }
    };

    if (typeof module !== 'undefined' && module.exports) {
        module.exports = FxImageZoom;
    } else {
        global.FxImageZoom = FxImageZoom;
    }
})(typeof window !== 'undefined' ? window : this);
