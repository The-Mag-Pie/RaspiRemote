var term = new Terminal();
var fitAddon = new FitAddon.FitAddon();
var attachAddon = undefined;
var sock = undefined;

term._publicOptions.fontSize = 12;

addEventListener("resize", () => fitAddon.fit());

function Initialize() {
    term.open(document.getElementById('terminal'));

    sock = new WebSocket('ws://localhost:8880/shell');
    // send any character to load shell data that has been retrieved before console initialization
    sock.onopen = () => setTimeout(() => sock.send("\x08"), 200);

    attachAddon = new AttachAddon.AttachAddon(sock);

    term.loadAddon(attachAddon);
    term.loadAddon(fitAddon);

    fitAddon.fit();

    // keyboard on Android is not functioning properly with xterm in some cases so some workarounds should be applied
    if (navigator.userAgent.includes("Android")) {
        configureTermForAndroid();
    }
}

function configureTermForAndroid() {
    // configure password input and xterm textarea elements to disable text composition system
    // input with type password disables the text composition system

    addEventListener("click", () => inputElem.focus());
    addEventListener("keydown", handleKeyDownEvent);

    let inputElem = document.getElementById("hiddenInput");
    inputElem.addEventListener("input", handleInputEvent);

    let inputElemForm = document.getElementById("hiddenInputForm");
    inputElemForm.addEventListener("submit", () => sock.send("\x0D"));

    let xtermTextareaElem = document.getElementsByTagName("textarea")[0];
    xtermTextareaElem.addEventListener("input", () => inputElem.focus());
    xtermTextareaElem.addEventListener("copy", () => inputElem.focus());
    xtermTextareaElem.addEventListener("paste", () => inputElem.focus());
}

function handleInputEvent(e) {
    if (e.inputType == "insertText") {
        sock.send(e.data);
    }
}

function handleKeyDownEvent(e) {
    if (e.key != "Unidentified") {
        e.preventDefault();

        if (e.code.startsWith("Key")) {
            sock.send(e.key);
        }
        else {
            sock.send(String.fromCharCode(e.keyCode));
        }
    }
}
