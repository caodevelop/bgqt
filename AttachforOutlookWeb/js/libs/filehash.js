!function (win, sparkMD5) {
    var Md5File = function (options) {
        this._init(options);
        this.fileMd5Hash();
    };
    Md5File.prototype = {
        _init: function (opts) {
            this.file = opts.file;
            this.fileSliceLength = opts.fileSliceLength || 1024 * 1024;
            this.chunks = opts.chunks || 10;
            this.chunkSize = parseInt(this.file.size / this.chunks, 10);
            this.currentChunk = 0;
            this.md5Complete = opts.md5Complete;
            this.spark = new sparkMD5.ArrayBuffer();
            this.sparks = [];
        },
        fileMd5Hash: function () {
            this._readFileMd5Hash();
        },
        _readFileMd5Hash: function () {
            var self = this;
            var start = self.currentChunk * self.chunkSize;
            var end = Math.min(start + self.chunkSize, self.file.size);
            if (self.currentChunk < self.chunks) {
                end = start + self.fileSliceLength;
            }

            var fileReader = new FileReader();
            fileReader.onload = function (e) {
                var spark = new sparkMD5.ArrayBuffer();
                spark.append(e.target.result);
                var tempSpark = spark.end();
                self.sparks.push(tempSpark);
                self.currentChunk++;
                if (self.currentChunk < self.chunks) {
                    self._readFileMd5Hash();
                } else {
                    var endSparkStr = self.sparks.join('');
                    self.md5Complete && typeof self.md5Complete == 'function' && self.md5Complete(endSparkStr);
                }
            };
            fileReader.readAsArrayBuffer(self.file.slice(start, end));
        }
    };
    win.md5File = function (options) {
        return new Md5File(options);
    }
}(window, SparkMD5);