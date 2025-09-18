var aeon = window.aeon || {};

aeon.flightInfo = function (initInfo) {
    var that = this;

    that.feeInfo = {
        INTERNATIONAL: {
            DEFAULT: 200000,
            ASIAN:120000
        },
        DOMESTIC: {
            ECONOMY: 30000,
            PREMIUM: 60000,
            BUSINESS: 60000
        }
    };

    that.getTicketPrice = function () {
        let price = 0;
        if (!$.isEmptyObject(that.pricedItineraries) && !$.isEmptyObject(that.pricedItineraries.airItineraryPricingInfo)) {
            let passengerFare = that.pricedItineraries.airItineraryPricingInfo.adultFare.passengerFare;
            let baseFare = passengerFare.baseFare.amount * 1;
            let serviceTax = passengerFare.serviceTax.amount * 1;
            price = baseFare + serviceTax + that.getTicketFee();
        }
        return price;
    };

    that.getTicketFee = function () {
        let fee = 0;
        if (!$.isEmptyObject(that.pricedItineraries)) {
            if (that.flightType == 'DOMESTIC') {
                fee = that.feeInfo[that.flightType][that.pricedItineraries.cabinClassName];
            }
            else {
                //VN	//Việt Nam
                //SG	//Singapore
                //BN	//Brunei
                //KH	//Campuchia
                //TL 	//Dong timor
                //LA 	//Lao
                //MY	//Malaysia
                //MM	//Myanmar
                //PH    //Philippines
                //TH 	//Thái Lan
                let asianCountries = ["VN", "SG", "BN", "KH", "TL", "MY", "MM", "PH", "TH"];
                if (asianCountries.indexOf(that.destinationCountryCode) >= 0) {
                    fee = that.feeInfo[that.flightType]["ASIAN"];
                }
                else {
                    fee = that.feeInfo[that.flightType]["DEFAULT"];
                }
            }
        }
        return fee;
    };

    that.init = function () {
        if (!$.isEmptyObject(initInfo)) {
            that = $.extend(that, initInfo);
        }
    };

    that.init();
    return that;
}

aeon.flightInfoArray = function (initInfo) {
    var that = this;
    that.ticketInfos = new Array();

    that.add = function (ticketInfo) {
        that.ticketInfos.push(new aeon.flightInfo(ticketInfo));
    }

    that.getTotalPrice = function (btaType) {
        let price = 0;

        try {
            let directFlight = that.getDirectFlight();
            let returnFlight = that.getReturnFlight();

            //ROUND TRIP: Check return trip ticket price only
            if (!$.isEmptyObject(directFlight) && !$.isEmptyObject(returnFlight)) {
                if (btaType == 3) {
                    //domestic
                    price = directFlight.getTicketPrice() + returnFlight.getTicketPrice();
                }
                else if (btaType == 2) {
                    //international
                    price = returnFlight.getTicketPrice();
                }
            }
            else {
                 //ONE WAY: Check price of that ticket
                if (!$.isEmptyObject(directFlight)) {
                    price = directFlight.getTicketPrice();
                }
                else if (!$.isEmptyObject(returnFlight)){
                    price = returnFlight.getTicketPrice();
                }
            }
        } catch (e) {
            price = 0;
        }
        return price;
    };

    that.getDirectFlight = function () {
        let returnValue = null;
        returnValue = $(that.ticketInfos).filter(function (cIndex, cItem) {
            if (!$.isEmptyObject(cItem) && cItem.directFlight) {
                return true;
            }
        });
        return !$.isEmptyObject(returnValue) ? returnValue[0] : null;
    };

    that.getReturnFlight = function () {
        let returnValue = null;
        returnValue = $(that.ticketInfos).filter(function (cIndex, cItem) {
            if (!$.isEmptyObject(cItem) && !cItem.directFlight) {
                return true;
            }
        });
        return !$.isEmptyObject(returnValue) ? returnValue[0] : null;
    };

    that.setDirectFlight = function (flightInfo) {
        for (var i = 0; i < that.ticketInfos.length; i++) {
            let cItem = that.ticketInfos[i];
            if (!$.isEmptyObject(cItem) && cItem.directFlight) {
                that.ticketInfos[i] = flightInfo;
            }
        }
    };

    that.setReturnFlight = function (flightInfo) {
        for (var i = 0; i < that.ticketInfos.length; i++) {
            let cItem = that.ticketInfos[i];
            if (!$.isEmptyObject(cItem) && !cItem.directFlight) {
                that.ticketInfos[i] = flightInfo;
            }
        }
    };

    that.mergeWithOldTicketInfo = function (oldTicketInfo) {
        if (!$.isEmptyObject(oldTicketInfo) && oldTicketInfo instanceof aeon.flightInfoArray) {
            let newDirectFlight = that.getDirectFlight();
            let oldDirectFlight = oldTicketInfo.getDirectFlight();
            if (!$.isEmptyObject(newDirectFlight)) {
                newDirectFlight.id = !$.isEmptyObject(oldDirectFlight) ? oldDirectFlight.id : null;
                that.setDirectFlight(newDirectFlight);
            }

            let newReturnFlight = that.getReturnFlight();
            let oldReturnFlight = oldTicketInfo.getReturnFlight();
            if (!$.isEmptyObject(newReturnFlight)) {
                newReturnFlight.id = !$.isEmptyObject(oldReturnFlight) ? oldReturnFlight.id : null;
                that.setReturnFlight(newReturnFlight);
            }
        }
        return that;
    };

    that.init = function () {
        if (!$.isEmptyObject(initInfo)) {
            $(initInfo).map(function (cIndex, cTicket) {
                that.ticketInfos.push(new aeon.flightInfo(cTicket));
            });
        }
    };

    that.init();

    return that;
}