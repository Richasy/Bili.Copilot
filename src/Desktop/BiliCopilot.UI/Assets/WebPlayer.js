let isFullWindowClicked = false;
let isFullScreenClicked = false;
let isMiniViewClicked = false;
const isFullWindow = `isFullWindow`;
const isFullScreen = `isFullScreen`;
const isMiniView = `isMiniView`;

function tryFullWindow() {
    if (isFullWindow && !isFullWindowClicked) {
        var div = document.querySelector('.bpx-player-ctrl-btn.bpx-player-ctrl-web');
        if (div) {
            div.click();
            isFullWindowClicked = true;
        }
    }
}

function tryFullScreen() {
    if (isFullScreen && !isFullScreenClicked) {
        var div = document.querySelector('.bpx-player-ctrl-btn.bpx-player-ctrl-full');
        if (div) {
            div.click();
            isFullScreenClicked = true;
        }
    }
}

function tryMiniView() {
    if (isMiniView && !isMiniViewClicked) {
        var div = document.querySelector('.bpx-player-ctrl-btn.bpx-player-ctrl-pip');
        if (div) {
            div.click();
            isMiniViewClicked = true;
        }
    }
}

const observer = new MutationObserver(function (mutationsList, observer) {
    let isMatched = false;
    for (let mutation of mutationsList) {
        if (mutation.type === 'childList') {
            tryFullWindow();
            tryFullScreen();
            tryMiniView();
        }
    }
});

observer.observe(document.documentElement, { childList: true, subtree: true });
tryFullWindow();
tryFullScreen();
tryMiniView();