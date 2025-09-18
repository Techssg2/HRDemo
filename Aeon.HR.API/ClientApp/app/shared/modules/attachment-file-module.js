var attachmentFile = angular.module("attachmentfileModule", []);
attachmentFile.factory("attachmentFile", function(attachmentService, $window) {
    
    var FileSaver = $window;

    async function uploadFile(model) {
        let result = await attachmentService.getInstance().attachmentFile.upload(model).$promise;
        return result;
    }

    async function download(model) {
        console.log(FileSaver);
        let result = await attachmentService.getInstance().attachmentFile.download(model).$promise;
        return result;
    }

    async function downloadAndSaveFile(model) {
        let result = await attachmentService.getInstance().attachmentFile.download(model).$promise;
        // return result;
        if (result.isSuccess) {
            let content = convertByteArrayToUint8Array(result.object.content);
            let newData = new Blob([content], { type: result.object.type });
            saveAs(newData, result.object.fileName);

            // Công việc này thành công
            return true;
        } else {
            // Có lỗi trong quá trình làm công việc này
            return false;
        }
    }

    async function getFilePath(model) {
        let result = await attachmentService.getInstance().attachmentFile.getFilePath(model).$promise;
        return result;
    }

    async function get(model) {
        let result = await attachmentService.getInstance().attachmentFile.get(model).$promise;
        return result;
    }

    async function getImage(model) {
        let result = await attachmentService.getInstance().attachmentFile.getImage(model).$promise;
        return result;
    }

    async function deleteFile(model) {
        let result = await attachmentService.getInstance().attachmentFile.delete(model).$promise;
        return result;
    }

    return {
        uploadFile: uploadFile,
        download: download,
        downloadAndSaveFile: downloadAndSaveFile,
        getFilePath: getFilePath,
        get: get,
        getImage: getImage,
        deleteFile: deleteFile   
    }
});
