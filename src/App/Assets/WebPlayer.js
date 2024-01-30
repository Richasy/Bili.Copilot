let elements = ['.video-page-game-card-small', '.bili-header', '.ad-report', '.video-card-ad-small', '.vcd', '.link-navbar-ctnr'];

function hideElements() {
    elements.forEach(function (selector) {
        let element = document.querySelector(selector);
        if (element) {
            element.style.display = 'none';
        }
    })

    let header = document.getElementById("biliMainHeader");
    header.style.height = "12px";
}

// 创建 MutationObserver 对象
const observer = new MutationObserver(function (mutationsList, observer) {
    let isMatched = false;
    // 遍历每个发生变化的 mutation
    for (let mutation of mutationsList) {
        // 检查是否有新增的节点
        if (mutation.type === 'childList' && mutation.addedNodes.length > 0) {
            // 遍历新增的节点
            mutation.addedNodes.forEach(function (node) {
                // 检查新增的节点是否匹配 hideElements 中的选择器
                elements.forEach(function (selector) {
                    if (node.matches(selector)) {
                        // 调用 hideElements 方法
                        isMatched = true;
                        hideElements();
                    }
                });
            });
        }
    }

    if (isMatched) {
        observer.disconnect();
    }
});

let lastHideElementsTime = 0;

// 监听滚动事件
window.addEventListener('scroll', function () {
    // 获取当前时间戳
    const currentTime = Date.now();

    // 检查距离上次触发 hideElements 是否超过300ms
    if (currentTime - lastHideElementsTime >= 300) {
        // 更新上次触发 hideElements 的时间戳
        lastHideElementsTime = currentTime;

        // 调用 hideElements 方法
        hideElements();
    }
});

// 开始观察整个文档的DOM变化
observer.observe(document.documentElement, { childList: true, subtree: true });
hideElements();