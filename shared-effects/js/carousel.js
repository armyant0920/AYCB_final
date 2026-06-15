/* =============================================================
 * carousel.js — 圖片輪播特效（共用類別）
 * -------------------------------------------------------------
 * 來源：原專案 src/main/webapp/WEB-INF/views/js/Home.js (.adbox 輪播)
 * 改寫：移除 jQuery 依賴，包裝成可重複使用的 ES5/ES6 類別，
 *       直接相容 .NET MVC（放在 wwwroot/js），無需任何套件。
 *
 * 功能：自動播放、滑鼠移上暫停、左右箭頭、圓點切換、無限循環。
 *
 * ---------------------- 使用方式 -----------------------------
 * HTML（class 對應 effects.css）：
 *   <div class="fx-carousel" id="banner">
 *     <ul class="fx-carousel__slides">
 *       <li><img src="/images/a.jpg"></li>
 *       <li><img src="/images/b.jpg"></li>
 *       <li><img src="/images/c.jpg"></li>
 *     </ul>
 *     <ul class="fx-carousel__dots"></ul>            <!-- 圓點會自動產生 -->
 *     <button class="fx-carousel__btn fx-carousel__btn--prev">&#10094;</button>
 *     <button class="fx-carousel__btn fx-carousel__btn--next">&#10095;</button>
 *   </div>
 *
 * JS：
 *   <script src="/js/carousel.js"></script>
 *   <script>
 *     new FxCarousel('#banner', { interval: 3000 });
 *   </script>
 *
 * 在 .NET MVC Razor (_Layout.cshtml 或頁面) 中：
 *   <script src="~/js/carousel.js"></script>
 *   <script> new FxCarousel('#banner'); </script>
 *
 * 參數 options：
 *   interval   自動播放間隔毫秒，預設 2000
 *   autoplay   是否自動播放，預設 true
 *   pauseOnHover 滑鼠移上是否暫停，預設 true
 * ============================================================= */
(function (global) {
    'use strict';

    function FxCarousel(target, options) {
        // 允許傳入選擇器字串或 DOM 元素
        this.root = typeof target === 'string' ? document.querySelector(target) : target;
        if (!this.root) {
            console.warn('[FxCarousel] 找不到輪播容器：', target);
            return;
        }

        var opts = options || {};
        this.interval = opts.interval || 2000;
        this.autoplay = opts.autoplay !== false;
        this.pauseOnHover = opts.pauseOnHover !== false;

        this.slidesEl = this.root.querySelector('.fx-carousel__slides');
        this.slides = this.slidesEl ? this.slidesEl.children : [];
        this.dotsEl = this.root.querySelector('.fx-carousel__dots');
        this.prevBtn = this.root.querySelector('.fx-carousel__btn--prev');
        this.nextBtn = this.root.querySelector('.fx-carousel__btn--next');

        this.count = this.slides.length;
        this.index = 0;
        this.timer = null;

        if (this.count === 0) {
            console.warn('[FxCarousel] 沒有任何投影片');
            return;
        }

        this._buildDots();
        this._bindEvents();
        this._render();
        if (this.autoplay) this.start();
    }

    // 依投影片數量自動建立圓點
    FxCarousel.prototype._buildDots = function () {
        if (!this.dotsEl) return;
        this.dotsEl.innerHTML = '';
        var self = this;
        for (var i = 0; i < this.count; i++) {
            var dot = document.createElement('li');
            (function (idx) {
                dot.addEventListener('click', function () {
                    self.goTo(idx);
                });
            })(i);
            this.dotsEl.appendChild(dot);
        }
        this.dots = this.dotsEl.children;
    };

    FxCarousel.prototype._bindEvents = function () {
        var self = this;
        if (this.prevBtn) this.prevBtn.addEventListener('click', function () { self.prev(); });
        if (this.nextBtn) this.nextBtn.addEventListener('click', function () { self.next(); });

        if (this.pauseOnHover) {
            this.root.addEventListener('mouseenter', function () { self.stop(); });
            this.root.addEventListener('mouseleave', function () {
                if (self.autoplay) self.start();
            });
        }
    };

    // 將目前 index 反映到畫面（位移投影片、標示圓點）
    FxCarousel.prototype._render = function () {
        this.slidesEl.style.transform = 'translateX(' + (-100 * this.index) + '%)';
        if (this.dots) {
            for (var i = 0; i < this.dots.length; i++) {
                this.dots[i].classList.toggle('is-active', i === this.index);
            }
        }
    };

    FxCarousel.prototype.goTo = function (i) {
        this.index = (i + this.count) % this.count;   // 無限循環
        this._render();
    };

    FxCarousel.prototype.next = function () { this.goTo(this.index + 1); };
    FxCarousel.prototype.prev = function () { this.goTo(this.index - 1); };

    FxCarousel.prototype.start = function () {
        var self = this;
        this.stop();
        this.timer = setInterval(function () { self.next(); }, this.interval);
    };

    FxCarousel.prototype.stop = function () {
        if (this.timer) {
            clearInterval(this.timer);
            this.timer = null;
        }
    };

    // 匯出：支援 CommonJS 與全域 window
    if (typeof module !== 'undefined' && module.exports) {
        module.exports = FxCarousel;
    } else {
        global.FxCarousel = FxCarousel;
    }
})(typeof window !== 'undefined' ? window : this);
