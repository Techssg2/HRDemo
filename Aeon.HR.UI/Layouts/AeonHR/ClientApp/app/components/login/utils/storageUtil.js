Storage.prototype.setItemWithSafe = function (name, value, iMaxLength) {
    var removeStorageItem = function (strge, txtName) {
        let counter = 0;
        while (strge.getItem(name + "_SAFE_STORAGE_" + counter) != null) {
            strge.removeItem(name + "_SAFE_STORAGE_" + counter);
            counter++;
        }
    }

    if (value != null && value != undefined && $.type(value) == "string") {
        var setStorageValue = function (name, value) {
            this.setItem(name, value);
        }

        function chunkSubstr(str, size) {
            const numChunks = Math.ceil(str.length / size)
            const chunks = new Array(numChunks)

            for (let i = 0, o = 0; i < numChunks; ++i, o += size) {
                chunks[i] = str.substr(o, size)
            }

            return chunks
        }
        var valueArray = chunkSubstr(value, iMaxLength);
        removeStorageItem(this, name);

        $(document).data(name, value);
    }
    else {
        removeStorageItem(this, name);
        this.setItem(name, value);
        $(document).data(name, value);
    }
}

Storage.prototype.getItemWithSafe = function (name) {
    var returnValue = null;
    if (name != null && name != undefined && $.type(name) == "string") {
        returnValue = $(document).data(name);
    }
    return returnValue;
}