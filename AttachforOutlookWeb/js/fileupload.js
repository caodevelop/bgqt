!function (win) {
    'use strict';
    var FileUpload = function (options) {
        if (typeof options != 'object') {
            throw Error('options not is object');
        }
        this.init(options);
        if (this.autoUpload) {
            this.start();
        }
    };

    FileUpload.prototype = {
        init: function (opts) {
            this.file = opts.file;
            this.url = opts.url;
            this.compileFileUrl = opts.compileFileUrl;
            this.fileHashUrl = opts.fileHashUrl;
            this.autoUpload = opts.autoUpload || 0;
            this.chunk = opts.chunk || (10 * 1024 * 1024);
            this.chunks = Math.ceil(this.file.size / this.chunk);
            this.currentChunk = 0;
            this.taskName = opts.taskName || win.uuid();
            this.isUploading = false;
            this.isUploaded = false;
            this.upProgress = opts.upProgress; //上传进度
            this.upComplete = opts.upComplete; //上传完成
            this.timeHandle = opts.timeHandle; // 时间处理
            this.pauseCallback = opts.pauseCallback; //暂停时处理的回调
            this.timeInfo = {
                h: 0,
                m: 0,
                s: 0
            };
            this.md5FileHash = null;
            this.remoteMd5FileHash = opts.remoteMd5FileHash; //远程对比文件
            this.isSendCompleteFile = 0; //是否发送合并文件指令
            //this.getFileUploadParams = opts.getFileUploadParams;
            //上传参数
            this.folderID = opts.folderID;
            this.storageID = opts.storageID;
            this.storageUri = opts.storageUri;
            this.storageRelativePath = opts.storageRelativePath;
            this.context = opts.context;
            return this;
        },
        start: function () {
            var self = this;
            self.isUploading = true;
            //秒传判断
            if (!self.md5FileHash) {
                win.md5File({
                    file: self.file,
                    md5Complete: function (hash) {
                        self.md5FileHash = hash;
                        //后台进行判断hash值是否一致，如何一致则直接上传完成。
                        if (self.remoteMd5FileHash && typeof self.remoteMd5FileHash == 'function') {
                            self.remoteMd5FileHash(self.folderID, self.file.name, self.md5FileHash, function (fileInfo) {
                                if (fileInfo.Succeed) {
                                    self.initInterval();
                                    self.isUploaded = true;
                                    self.isUploading = false;
                                    self.upProgress && typeof self.upProgress == 'function' && self.upProgress(1, 1);
                                    if (self.upComplete && typeof self.upComplete == 'function') {
                                        if (self.time) win.clearInterval(self.time);
                                        self.upComplete(fileInfo);
                                    }
                                } else {
                                    if (!self.isUploaded) {
                                        self.taskName = fileInfo.TempID;
                                        self.currentChunk = fileInfo.ChunkIndex;
                                        self.initInterval();
                                        self._upload();
                                    }
                                }
                            });
                        }
                    }
                });
            } else {
                //上传
                if (!self.isUploaded) {
                    self.initInterval();
                    self._upload();
                }
            }
        },
        pause: function (error) {
            if (this.isUploading) {
                if (this.xhr) {
                    this.xhr.abort();
                    this.xhr = null;
                }
                this.isUploading = false;
                if (this.time) win.clearInterval(this.time);
                this.pauseCallback && this.pauseCallback(error);
            }
        },
        _initXhr: function () {
            var self = this;
            if (self.xhr) {
                self.xhr.abort();
            }
            self.xhr = new XMLHttpRequest();
            self.xhrLoad = function () {
                var rData = JSON.parse(self.xhr.responseText);
                if (self.xhr.status == 401 || self.xhr.status == 403) {
                    self.pause(401);
                } else if (self.xhr.status == 404) {
                    self.pause(404);
                } else if (self.xhr.status == 500) {
                    if (rData.error) {
                        self.pause(rData.error.ErrorCode);
                    } else {
                        self.pause(404);
                    }
                } else {
                    if (self.xhr.status == 200) {
                        if (rData.error) {
                            if (rData.error.ErrorCode == '3003') {
                                self.currentChunk = rData.error.ChunkIndex;
                                self._upload();
                            } else {
                                self.pause(rData.error.ErrorCode);
                            }
                            return;
                        }
                        if (self.end == self.file.size) {
                            self.isUploaded = true;
                            if (self.upComplete && typeof self.upComplete == 'function') {
                                if (self.time) win.clearInterval(self.time);
                                self.upComplete(rData.data);
                            }
                            self.isSendCompleteFile && self.compileFile();
                        } else {
                            if (self.upProgress && typeof self.upProgress == 'function') {
                                self.upProgress((self.currentChunk + 1), self.chunks);
                            }
                            self.currentChunk++;
                            self._upload();
                        }
                    } else {
                        self.pause(404);
                    }
                }
            };
            self.xhrError = function () {
                //console.log('xhr error');
                self.pause(404);
            };
            self.xhrAbort = function () {
                //console.log('xhr abort');
            };

            self.ontimeout = function () {
                self.pause(404);
            };

            self.xhr.onload = this.xhrLoad;
            self.xhr.onerror = this.xhrError;
            self.xhr.onabort = this.xhrAbort;
            self.xhr.ontimeout = this.ontimeout;
        },
        _upload: function () {
            var self = this;
            self.isUploading = true;
            self._initXhr();
            self.xhr.open('POST', self.url);
            //计算上传开始位置或结束位置
            self.begin = self.currentChunk * self.chunk;
            self.end = Math.min((self.begin + self.chunk), self.file.size);
            var blob = self.file.slice(self.begin, self.end, { type: 'text/plain' });
            var tempFileName = 'temp-' + self.currentChunk + '-' + self.taskName;
            var formData = new FormData();
            formData.append('FolderID', self.folderID);
            formData.append('StorageID', self.storageID);
            formData.append('StorageUri', self.storageUri);
            formData.append('StorageRelativePath', self.storageRelativePath);
            formData.append('HashCode', self.md5FileHash);
            formData.append('TotalChunks', self.chunks);
            formData.append('ChunkIndex', (self.currentChunk + 1));
            formData.append('FileName', self.file.name);
            formData.append('FileSize', self.file.size);
            formData.append('Identifier', self.taskName);
            formData.append('Data', blob, tempFileName);
            formData.append('Position', self.begin);

            self.xhr.send(formData);
        },
        compileFile: function () {
            var self = this;
            self._initXhr();
            self.xhrLoad = function () {
                console.log(self.xhr.responseText);
            };
            self.xhr.onload = self.xhrLoad;
            self.xhr.open('POST', self.compileFileUrl);
            var formData = new FormData();
            formData.append("fileName", (self.file.name));
            formData.append('taskName', self.taskName);
            self.xhr.send(formData);
        },
        initInterval: function () {
            var self = this;
            if (self.time) win.clearInterval(self.time);
            self.time = win.setInterval(function () {
                self.timeInfo.s++;
                if (self.timeInfo.s == 60) {
                    self.timeInfo.s = 0;
                    self.timeInfo.m++;

                    if (self.timeInfo.m == 60) {
                        self.timeInfo.m = 0;
                        self.timeInfo.h++;
                    }
                }
                self.timeHandle && self.timeHandle(self.formatStr());
            }, 1000);
        },
        formatStr: function () {
            var _ = this,
                sY = (_.timeInfo.h < 10) ? '0' + _.timeInfo.h : _.timeInfo.h,
                sM = (_.timeInfo.m < 10) ? '0' + _.timeInfo.m : _.timeInfo.m,
                sS = (_.timeInfo.s < 10) ? '0' + _.timeInfo.s : _.timeInfo.s;
            return sY + ':' + sM + ':' + sS;
        }
    };
    win.fileUpload = function (options) {
        return new FileUpload(options);
    };
}(window);
