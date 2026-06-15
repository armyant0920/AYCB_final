/* =============================================================
 * marquee.js — 文字跑馬燈（共用類別，可選用）
 * -------------------------------------------------------------
 * 純 CSS 版本（見 effects.css 的 .fx-marquee）已可運作；
 * 本檔提供 JS 版，額外支援「動態設定文字」「速度」「方向」，
 * 並可在內容不夠長時自動補滿，達成無縫捲動。
 *
 * ---------------------- 使用方式 -----------------------------
 * 方式 A — 純 CSS（最簡單，不需本檔）：
 *   <div class="fx-marquee" style="--fx-marquee-duration:12s">
 *     <span>最新消息：歡迎光臨本網站！</span>
 *   </div>
 *
 * 方式 B — 用 JS 類別動態建立：
 *   <div id="news"></div>
 *   <script src="/js/marquee.js"></script>
 *   <script>
 *     new FxMarquee('#news', { text: '最新消息：本週全館 9 折！', duration: 12 });
 *   </script>
 *
 * .NET MVC Razor：
 *   <script src="~/js/marquee.js"></script>
 *   <script> new FxMarquee('#news', { text: ViewBag.Notice }); </script>
 *
 * 參數 options：
 *   text      要捲動的文字（若 HTML 內已有內容可省略）
 *   duration  跑一圈的秒數，預設 15
 * ============================================================= */
(function (global) {
    'use strict';

    function FxMarquee(target, options) {
        this.root = typeof target === 'string' ? document.querySelector(target) : target;
        if (!this.root) {
            console.warn('[FxMarquee] 找不到容器：', target);
            return;
        }

        var opts = options || {};
        var text = opts.text;
        var duration = opts.duration || 15;

        this.root.classList.add('fx-marquee');
        this.root.style.setProperty('--fx-marquee-duration', duration + 's');

        // 若有傳入 text，建立內層 span；否則沿用既有子節點
        if (typeof text === 'string') {
            this.root.innerHTML = '<span></span>';
            this.root.firstChild.textContent = text;
        } else if (this.root.children.length === 0) {
            // 把純文字包進 span 以便套用動畫
            var span = document.createElement('span');
            span.textContent = this.root.textContent;
            this.root.innerHTML = '';
            this.root.appendChild(span);
        }
    }

    // 之後想換文字：marquee.setText('新內容')
    FxMarquee.prototype.setText = function (text) {
        var inner = this.root.querySelector('span') || this.root.firstElementChild;
        if (inner) inner.textContent = text;
    };

    if (typeof module !== 'undefined' && module.exports) {
        module.exports = FxMarquee;
    } else {
        global.FxMarquee = FxMarquee;
    }
})(typeof window !== 'undefined' ? window : this);
