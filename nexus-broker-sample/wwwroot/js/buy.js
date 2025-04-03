/*
This Javascript file is used for all the broker buy functions needed to make the buy pages more dynamic.
This makes use of the AjaxController exposed API endpoints.
*/
$(function () {
    var relativeImagePath = '/images/payments/';
    var accountValid = false;
    var availablePaymentMethods;
    var minAmount = 0;
    var maxAmount = 0;
    var currentDCCode;

    var LimitReasonEnum = {
        LowBalance: "LowBalance",
        MonthlyBuyLimit: "MonthyBuyLimit",
        DailyBuyLimit: "DailyBuyLimit",
        NoReferencePrice: "NoReferencePrice",
        NoBalance: "NoBalance",
        UnidentifiedBusiness: "UnidentifiedBusiness",
        NewUnsettled: "NewUnsettled",
        Incasso: "Incasso",
        MissingBuyTransaction: "MissingBuyTransaction",
        RequestedHigherThanMaximum: "RequestedHigherThanMaximum",
        RequestedLowerThanMinimum: "RequestedLowerThanMinimum",
        RequestedHigherThanAvailableAmount: "RequestedHigherThanAvailableAmount"
    };

    function jsonRequest(config) {
        var ajaxConfig = {
            type: config.type,
            data: JSON.stringify(config.data),
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            headers: {}
        };
        if (config.error) {
            ajaxConfig.error = config.error;
        }
        return $.ajax(config.url, ajaxConfig);
    }

    function getAccountCode() {
        return $('input[name=AccountCode]').val().trim();
    }

    function firePaymentMethodEvent() {
        var paymentMethodCode = $('select[name=PaymentMethodCode] :selected').val();
        var accountCode = getAccountCode();

        jsonRequest({
            url: "/api/ajax/checkbuylimits?accountId=" + accountCode + "&paymentMethodCode=" + paymentMethodCode,
            type: "get",
            data: {}
        }).done(function (data) {
            maxAmount = data.remainingDailyLimit;
            minAmount = data.minimumAmount;
            updateAmountLimitText();
            updateLimitReasonHeaders(data.limitReasons);
            updateForm();
        });
    }

    function updateForm() {
        if (accountValid === false) return;

        var paymentMethodCode = $('select[name=PaymentMethodCode] :selected').val();

        $.each(availablePaymentMethods, function (index, method) {
            if (method.paymentMethodCode === paymentMethodCode) {
                var imagefile = relativeImagePath + method.paymentTypeCode + '.png';
                $('#PaymentImage').prop('src', imagefile);
            }
        });

        var amount = $('input[name=Amount]').val();

        var postData = {
            AccountCode: getAccountCode(),
            PaymentMethodCode: paymentMethodCode,
            Amount: Number(amount),
            DCCode: currentDCCode,
            IncludeFees: true
        };

        if (amount < minAmount || amount > maxAmount) {
            var v = 0.0;
            $("#curAmountLimit").css('color', 'red');
            $("input[name=Amount]").css('border-color', 'red');
            $('.btn.btn-send').prop('disabled', true);
            $('#BTCAmount').text(v.toFixed(8));
            $('#BTCBuyPrice').text('');
            $('#EuroBankFee').text(v.toFixed(2));
            $('#EuroServiceFee').text(v.toFixed(2));
            $('#EuroNetworkFee').text(v.toFixed(2));
            $('#TotalEuro').text(v.toFixed(2));
            return;
        }

        $("#curAmountLimit").css('color', 'green');
        $("input[name=Amount]").css('border-color', '#d4d7de');
        $('.btn.btn-send').prop('disabled', false);

        jsonRequest({
            url: "/api/ajax/simulatebuy",
            type: "post",
            data: postData,
            error: function (xhr, status, error) {
                if (error && error.limitReasons && error.limitReasons.length > 0) {
                    updateLimitReasonHeaders(error.limitReasons);
                    return;
                }
            }
        }).done(function (data) {
            if (!data) { return; }

            $('.currency-code').text(data.currencyCode);
            $('.dc-code').text(data.cryptoCode);
            $('#BTCAmount').text(data.cryptoAmount.toFixed(8));
            $('#BTCBuyPrice').text('(' + data.cryptoBuyPriceBeforeFee.toFixed(2) + ' ' + data.currencyCode + '/' + data.cryptoCode + ')');
            $('#EuroBankFee').text(data.currencyBankFee.toFixed(2));
            $('#EuroServiceFee').text(data.currencyServiceFee.toFixed(2));
            $('#EuroNetworkFee').text(data.currencyNetworkFee.toFixed(2));
            $('#TotalEuro').text(data.totalCurrency.toFixed(2));

            var submit = $('.btn.btn-send');
            if (data.cryptoAmount.toFixed(8) > 0.0) {
                submit.removeProp('disabled').removeAttr('disabled');
            } else {
                submit.prop('disabled', 'disabled');
            }
        });
    }

    function checkAccount() {
        var accountCode = getAccountCode();

        if (accountCode.length < 8) {
            return;
        }

        jsonRequest({
            url: "/api/ajax/checkbuyinfo/" + accountCode,
            type: "get",
            data: {}
        }).done(function (data) {

            if (data.dcCode !== cryptoCode) {

                jsonRequest({
                    url: "/api/ajax/getsupportedcrypto/" + data.dcCode,
                    type: "get",
                    data: {}
                }).done(function (supportedCrypto) {
                    const prettyCrypto = supportedCrypto.name

                    if (!redirecting) {
                        if (confirm(`This appears to be a ${prettyCrypto} account code, do you want to switch the form to ${prettyCrypto}?`)) {
                            window.location.href = "/buy/" + supportedCrypto.route + "?id=" + getAccountCode();
                            redirecting = true;
                        } else {
                            var submit2 = $('.btn.btn-send');
                            $('#wrong-coin').prop('visible', 'block').show();
                            submit2.prop('disabled', 'disabled');
                            $('input[name=AccountCode]').removeProp('disabled').removeAttr('disabled');
                            accountValid = false;
                        }
                    }
                });
            }
            else {
                updateAccountCode(data);

                currentDCCode = data.dcCode;

                if (data.accountValid) {
                    $('input[name=Amount]').focus();
                    updatePayMethods(data.paymentMethods);
                    updateWarningHeaders(data.paymentPending, data.coolingDown);
                    minAmount = data.limits.minimumAmount;
                    maxAmount = data.limits.remainingDailyLimit;
                    updateAmountLimitText();
                    updateLimitReasonHeaders(data.limitReasons);
                    updateForm();

                    $('input[name=Currency]').val(data.currency);
                }
            }
        });
    }

    function updateLimitReasonHeaders(limitReasons) {

        if (limitReasons && limitReasons.length > 0) {
            $('#limit-reason-title').prop('visible', 'block').show();

            if (limitReasons.includes(LimitReasonEnum.LowBalance)) {
                $('#low-balance').prop('visible', 'block').show();
            }
            if (limitReasons.includes(LimitReasonEnum.DailyBuyLimit)) {
                $('#daily-limit').prop('visible', 'block').show();
            }
            if (limitReasons.includes(LimitReasonEnum.MonthlyBuyLimit)) {
                $('#monthly-limit').prop('visible', 'block').show();
            }
            if (limitReasons.includes(LimitReasonEnum.NoReferencePrice)) {
                $('#reference-price').prop('visible', 'block').show();
            }
            if (limitReasons.includes(LimitReasonEnum.NoBalance)) {
                $('#no-balance').prop('visible', 'block').show();
            }
            if (limitReasons.includes(LimitReasonEnum.UnidentifiedBusiness)) {
                $('#unidentified-business').prop('visible', 'block').show();
            }
            if (limitReasons.includes(LimitReasonEnum.NewUnsettled)) {
                $('#new-unsettled').prop('visible', 'block').show();
            }
            if (limitReasons.includes(LimitReasonEnum.Incasso)) {
                $('#incasso').prop('visible', 'block').show();
            }
            if (limitReasons.includes(LimitReasonEnum.MissingBuyTransaction)) {
                $('#missing-buy').prop('visible', 'block').show();
            }
            if (limitReasons.includes(LimitReasonEnum.RequestedHigherThanMaximum)) {
                $('#high-request').prop('visible', 'block').show();
            }
            if (limitReasons.includes(LimitReasonEnum.RequestedLowerThanMinimum)) {
                $('#low-request').prop('visible', 'block').show();
            }
            if (limitReasons.includes(LimitReasonEnum.RequestedHigherThanAvailableAmount)) {
                $('#available-lower-than-request').prop('visible', 'block').show();
            }
        };
    };

    function updateWarningHeaders(paymentPending, coolingDown) {
        if (paymentPending) {
            $('#buy-notice-payment-pending').show();
        }
        else if (coolingDown) {
            $('#buy-notice-cooldown').show();
        }
    }

    function updateAmountLimitText() {
        var content = "min: " + minAmount + " max: " + maxAmount;
        $("#curAmountLimit").html(content);
    }

    // Update account input field according to web response
    function updateAccountCode(data) {
        if (data.accountValid === true) {
            $('#check-account').prop('disabled', 'disabled').hide();
            $('#wrong-coin').prop('visible', 'none').hide();
            $('#non-active').prop('visible', 'none').hide();
            $('#new-account').prop('visible', 'none').hide();
            $('#new-personal').prop('visible', 'none').hide();
            $('#trusted-personal').prop('visible', 'none').hide();
            $('#verified-personal').prop('visible', 'none').hide();
            $('#new-business').prop('visible', 'none').hide();
            $('#verified-business').prop('visible', 'none').hide();
            $('#business-limited').prop('visible', 'none').hide();
            $('#personal-limited').prop('visible', 'none').hide();
            $('#new-accounts-fasttrack').prop('visible', 'none').hide();
            $('#new-business-verification').prop('visible', 'none').hide();

            if (data.isBusiness === true) {
                if (data.accountType === "Identified") {
                    $('#verified-business').prop('visible', 'block').show();
                }
                else if (data.accountType === "New") {
                    $('#new-business').prop('visible', 'block').show();
                }
            }
            else {
                if (data.accountType === "Identified") {
                    $('#verified-personal').prop('visible', 'block').show();
                }
                else if (data.accountType === "Trusted") {
                    $('#trusted-personal').prop('visible', 'block').show();
                }
                else if (data.accountType === "New") {
                    $('#new-personal').prop('visible', 'block').show();
                }
            }
            $('input[name=AccountCode]').prop('readonly', 'readonly');
            updateBuyFormInterval = setInterval(function () { refreshFormData(); }, 60000);
            accountValid = true;
        }
        else {
            var submit = $('.btn.btn-send');
            $('#non-active').prop('visible', 'block').show();
            submit.prop('disabled', 'disabled');
            $('input[name=AccountCode]').removeProp('disabled').removeAttr('disabled');
            accountValid = false;
        }
    }

    var redirecting = false;

    // Update Payment method input field with the new values
    function updatePayMethods(PaymentMethods) {
        availablePaymentMethods = PaymentMethods;

        var paymethComboBox = $('select[name=PaymentMethodCode]');

        //Clean the content
        paymethComboBox.empty();

        $.each(PaymentMethods, function (index, paymethod) {

            if (index === 0) {
                paymethComboBox.append($('<option></option>').val(paymethod.paymentMethodCode).html(paymethod.paymentTypeDisplay).prop('selected', 'selected'));
            }
            else {
                paymethComboBox.append($('<option></option>').val(paymethod.paymentMethodCode).html(paymethod.paymentTypeDisplay));
            }
        });
        $('select[name=PaymentMethodCode]').selectric('refresh');


    }

    $('input[name=AccountCode]').on("input keyup change", function () {
        $('input[name=AccountCode]').val(getAccountCode());
        checkAccount();
    });

    $('.updateBitcoin').on("input keyup change", function () { updateForm(); });

    /// pressing enter while typing accountcode will click CheckAccount button
    $('input[name=AccountCode]').on('keypress', function (event) {
        if (event.which === '13') {
            event.preventDefault();
        }
    });

    $('select[name=PaymentMethodCode]').on('change', function () {
        firePaymentMethodEvent();
    });

    function refreshFormData() {
        var accountCode = getAccountCode();
        jsonRequest({
            url: "/api/ajax/checkbuyinfo/" + accountCode,
            type: "get",
            data: {}
        }).done(function (data) {
            refreshingForm = true;
            currentDCCode = data.dcCode;
            updateForm();
            refreshingForm = false;
        });
    }

    checkAccount();
});