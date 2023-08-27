//Copyright (c) 2017, The xterm.js authors (https://github.com/xtermjs/xterm.js)

//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in
//all copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//THE SOFTWARE.

!function (e, t) { "object" == typeof exports && "object" == typeof module ? module.exports = t() : "function" == typeof define && define.amd ? define([], t) : "object" == typeof exports ? exports.AttachAddon = t() : e.AttachAddon = t() }(self, (function () { return (() => { "use strict"; var e = {}; return (() => { var t = e; function s(e, t, s) { return e.addEventListener(t, s), { dispose: () => { s && e.removeEventListener(t, s) } } } Object.defineProperty(t, "__esModule", { value: !0 }), t.AttachAddon = void 0, t.AttachAddon = class { constructor(e, t) { this._disposables = [], this._socket = e, this._socket.binaryType = "arraybuffer", this._bidirectional = !(t && !1 === t.bidirectional) } activate(e) { this._disposables.push(s(this._socket, "message", (t => { const s = t.data; e.write("string" == typeof s ? s : new Uint8Array(s)) }))), this._bidirectional && (this._disposables.push(e.onData((e => this._sendData(e)))), this._disposables.push(e.onBinary((e => this._sendBinary(e))))), this._disposables.push(s(this._socket, "close", (() => this.dispose()))), this._disposables.push(s(this._socket, "error", (() => this.dispose()))) } dispose() { for (const e of this._disposables) e.dispose() } _sendData(e) { this._checkOpenSocket() && this._socket.send(e) } _sendBinary(e) { if (!this._checkOpenSocket()) return; const t = new Uint8Array(e.length); for (let s = 0; s < e.length; ++s)t[s] = 255 & e.charCodeAt(s); this._socket.send(t) } _checkOpenSocket() { switch (this._socket.readyState) { case WebSocket.OPEN: return !0; case WebSocket.CONNECTING: throw new Error("Attach addon was loaded before socket was open"); case WebSocket.CLOSING: return console.warn("Attach addon socket is closing"), !1; case WebSocket.CLOSED: throw new Error("Attach addon socket is closed"); default: throw new Error("Unexpected socket state") } } } })(), e })() }));
//# sourceMappingURL=xterm-addon-attach.js.map