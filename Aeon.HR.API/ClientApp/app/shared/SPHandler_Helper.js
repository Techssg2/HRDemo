var isNullOrUndefined = function (obj) {
	return obj == null || obj == undefined;
}


var callHandler = function (param, callBack) {
	var deferred = new $.Deferred();
	//var url = "/_layouts/15/AeonHR/Handler/Common.ashx";
	var url = "/HR/Handler/Common.ashx";
	try {
		$.ajax({
			contentType: "application/json; charset=utf-8",
			url: url,
			data: param,
			cache: false,
			async: false,
			success: function (data) {
				try {
					deferred.resolve(data);
					if (!isNullOrUndefined(callBack) && typeof (callBack) === "function") {
						callBack(data);
					}
				}
				catch (e) {
					if (!isNullOrUndefined(callBack) && typeof (callBack) === "function") {
						callBack(null);
					}
					deferred.reject();
				}
			},
			error: function (e) {
				if (!isNullOrUndefined(callBack) && typeof (callBack) === "function") {
					callBack(null);
				}
				deferred.reject();
			},
		});
	}
	catch (e) {
		console.error(e);
		deferred.reject();
	}
	return deferred;
}
