/* =============================================================
 * image-preview.js — 圖片上傳即時預覽（共用類別）
 * -------------------------------------------------------------
 * 來源：原專案 src/main/webapp/WEB-INF/views/js/preview.js
 * 改寫：移除 jQuery 依賴，包裝成可重複使用的類別。
 *
 * 功能：使用者在 <input type="file"> 選好圖片後，
 *       立即把圖片顯示到指定的 <img> 元素，不需先上傳。
 *
 * ---------------------- 使用方式 -----------------------------
 * HTML：
 *   <input type="file" id="imageFile" accept="image/*">
 *   <img id="imageDemo" alt="預覽圖" />
 *
 * JS：
 *   <script src="/js/image-preview.js"></script>
 *   <script>
 *     new FxImagePreview('#imageFile', '#imageDemo');
 *   </script>
 *
 * .NET MVC Razor：
 *   <script src="~/js/image-preview.js"></script>
 *   <script> new FxImagePreview('#imageFile', '#imageDemo'); </script>
 *
 * 參數：
 *   input    file input 的選擇器或元素
 *   preview  要顯示預覽的 <img> 選擇器或元素
 * ============================================================= */
(function (global) {
    'use strict';

    function FxImagePreview(input, preview) {
        this.input = typeof input === 'string' ? document.querySelector(input) : input;
        this.preview = typeof preview === 'string' ? document.querySelector(preview) : preview;

        if (!this.input || !this.preview) {
            console.warn('[FxImagePreview] 找不到 input 或 preview 元素');
            return;
        }

        var self = this;
        this.input.addEventListener('change', function () {
            var file = self.input.files && self.input.files[0];
            if (!file) return;

            var reader = new FileReader();
            reader.onload = function (e) {
                self.preview.setAttribute('src', e.target.result);
            };
            reader.readAsDataURL(file);
        });
    }

    if (typeof module !== 'undefined' && module.exports) {
        module.exports = FxImagePreview;
    } else {
        global.FxImagePreview = FxImagePreview;
    }
})(typeof window !== 'undefined' ? window : this);
