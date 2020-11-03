
$(function () {
    var relativeImagePath = '/images/payments/';
    var accountValid = false;
    var refreshingForm = false;
    var availablePaymentMethods;
    var updateBuyFormInterval;
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
        NewCustomerNoFotoID: "NewCustomerNoFotoID",
        NewUnsettled: "NewUnsettled",
        Incasso: "Incasso",
        HighRisk: "HighRisk",
        MissingBuyTransaction: "MissingBuyTransaction",
        RequestedHigherThanMaximum: "RequestedHigherThanMaximum",
        RequestedLowerThanMinimum: "RequestedLowerThanMinimum",
        RequestedHigherThanAvailableAmount: "RequestedHigherThanAvailableAmount"
    };

    function jsonRequest(config) {
        return $.ajax(config.url, {
            type: config.type,
            //contentType: "application/json",
            data: JSON.stringify(config.data), // JSON data goes here
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            headers: {
                //'RequestVerificationToken': tokenHeaderValue
            }
        });
    }

    function getAccountCode() {
        return $('input[name=AccountCode]').val().trim();
    }

    function updateForm() {
        if (accountValid === false) return;

        var paymentMethodCode = $('select[name=PaymentMethodCode] :selected').val();

        $.each(availablePaymentMethods, function (index, method) {
            if (method.paymentMethodCode === paymentMethodCode) {
                var imagefile = relativeImagePath + method.paymentTypeCode + '.png';
                $('#PaymentImage').prop('src', imagefile);
                minAmount = method.minAmount;
                maxAmount = method.maxAmount;
                setAmountRanges();
                $('input[name=Amount]').val(minAmount);
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
            url: "/api/ajax/bitcoinvalue",
            type: "post",
            data: postData
        }).done(function (data) {
            // old: (1.0/1.1)
            //$('.currency-code').text(data.currency);
            //$('.dc-code').text(data.dcCode);
            //$('#BTCAmount').text(data.info.dcAmount.toFixed(8));
            //$('#BTCBuyPrice').text('(' + data.info.dcBuyPriceBeforeFee.toFixed(2) + ' ' + data.currency + '/' + data.dcCode + ')');
            //$('#EuroBankFee').text(data.info.currencyBankFee.toFixed(2));
            //$('#EuroServiceFee').text(data.info.currencyServiceFee.toFixed(2));
            //$('#EuroNetworkFee').text(data.info.currencyNetworkFee.toFixed(2));
            //$('#TotalEuro').text(data.info.totalCurrency.toFixed(2));

            //var submit = $('.btn.btn-send');
            //if (data.info.dcAmount.toFixed(8) > 0.0) {
            //    submit.removeProp('disabled').removeAttr('disabled');
            //} else {
            //    submit.prop('disabled', 'disabled');
            //}

            // 1.2

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
            updateAccountCode(data);

            currentDCCode = data.dcCode;

            if (data.accountValid) {
                if (data.firstBuyStatus === 1) {
                    $('#sell-notice-needsuccessfulbuytransaction-beforefirstsell').show();
                }

                $('input[name=Amount]').focus();

                updatePayMethods(data.paymentMethods);
                updateWarningHeaders(data.paymentPending, data.coolingDown, data.needFotoID);
                updateLimitReasonHeaders(data.limitReasons);
                updateForm();

                $('input[name=Currency]').val(data.currency);
            }
        });
    }

    function updateLimitReasonHeaders(limitReasons) {

        if (limitReasons.length > 0) {
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
            if (limitReasons.includes(LimitReasonEnum.NewCustomerNoFotoID)) {
                $('#new-customer-no-foto').prop('visible', 'block').show();
            }
            if (limitReasons.includes(LimitReasonEnum.NewUnsettled)) {
                $('#new-unsettled').prop('visible', 'block').show();
            }
            if (limitReasons.includes(LimitReasonEnum.Incasso)) {
                $('#incasso').prop('visible', 'block').show();
            }
            if (limitReasons.includes(LimitReasonEnum.HighRisk)) {
                $('#high-risk').prop('visible', 'block').show();
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

    function updateWarningHeaders(paymentPending, coolingDown, needFotoID) {
        if (paymentPending) {
            $('#buy-notice-payment-pending').show();
        }
        else if (coolingDown) {
            $('#buy-notice-cooldown').show();
        }
        else if (needFotoID) {
            $('#buy-notice-need-fotoid').show();
        }
    }

    function updateAmountRange() {
        updateAmountLimitText();
    }

    function updateAmountLimitText() {
        var content = "min: " + minAmount + " max: " + maxAmount;
        $("#curAmountLimit").html(content);
    }

    function setAmountRanges() {
        var paymentMethodCode = $('select[name=PaymentMethodCode] :selected').val();
        updateAmountLimitText();
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

            if (data.isPromoCodeAllowed === true) {
                $('.promocode-disallowed').show();
            }
            else {
                $('.promocode-disallowed').hide();
            }
            if (data.isBusiness === true) {
                if (data.accountType === "Identified") {
                    $('#verified-business').prop('visible', 'block').show();
                    if (data.highRisk === true) {
                        $('#business-limited').show();
                    }
                }
                else if (data.accountType === "New") {
                    $('#new-business').prop('visible', 'block').show();
                    if (data.firstBuyStatus === 0) {
                        $('#new-business-verification').show();
                    }
                }
            }
            else {
                if (data.accountType === "Identified") {
                    $('#verified-personal').prop('visible', 'block').show();
                    if (data.highRisk === true) {
                        $('#personal-limited').show();
                    }
                }
                else if (data.accountType === "Trusted") {
                    $('#trusted-personal').prop('visible', 'block').show();
                    if (data.highRisk === true) {
                        $('#personal-limited').show();
                    }
                }
                else if (data.accountType === "New") {
                    $('#new-personal').prop('visible', 'block').show();
                    if (data.highRisk === true) {
                        $('#personal-limited').show();
                    }
                    else {
                        if (data.firstBuyStatus === 0) {
                            $('#new-accounts-fasttrack').show();
                        }
                    }
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

        if (data.dcCode !== cryptoCode) {

            const cryptoNames = {
                BTC: 'bitcoin',
                BCH: 'bitcoincash',
                ETH: 'ethereum',
                LTC: 'litecoin',
                XLM: 'lumen'
            }

            const cryptoPrettyNames = {
                BTC: 'Bitcoin',
                BCH: 'Bitcoin Cash',
                ETH: 'Ethereum',
                LTC: 'Litecoin',
                XLM: 'Lumen'
            }

            const crypto = cryptoNames[data.dcCode]
            const prettyCrypto = cryptoPrettyNames[data.dcCode]

            if (!redirecting) {
                if (confirm(`This appears to be a ${prettyCrypto} account code, do you want to switch the form to ${prettyCrypto}?`)) {
                    window.location.href = "/buy/" + crypto + "?id=" + getAccountCode();
                    redirecting = true;
                } else {
                    var submit2 = $('.btn.btn-send');
                    $('#wrong-coin').prop('visible', 'block').show();
                    submit2.prop('disabled', 'disabled');
                    $('input[name=AccountCode]').removeProp('disabled').removeAttr('disabled');
                    accountValid = false;
                }
            }
        }
    }

    var redirecting = false;

    // Update Payment method input field with the new values
    function updatePayMethods(PaymentMethods) {
        availablePaymentMethods = PaymentMethods;

        var selectedMethodCode = $("select[name=PaymentMethodCode] option:selected").val();

        var paymethComboBox = $('select[name=PaymentMethodCode]');

        //Clean the content
        paymethComboBox.empty();

        $.each(PaymentMethods, function (index, paymethod) {
            //var option = $('<option></option>').val(paymethod.Id).html(paymethod.Name);

            if (paymethod.paymentMethodCode === selectedMethodCode) {
                paymethComboBox.append($('<option></option>').val(paymethod.paymentMethodCode).html(paymethod.paymentTypeName).prop('selected', 'selected'));

                // TODO: enable this back
                minAmount = paymethod.minAmount;
                maxAmount = paymethod.maxAmount;
                //minAmount = 2;
                //maxAmount = 100;
                setAmountRanges();
                $('input[name=Amount]').val(minAmount);
            }
            else {
                paymethComboBox.append($('<option></option>').val(paymethod.paymentMethodCode).html(paymethod.paymentTypeName));
            }
        });

        // jcf.refresh($('select[name=PaymentMethodCode]'));
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

    function refreshFormData() {
        var accountCode = getAccountCode();
        jsonRequest({
            url: "/api/ajax/checkbuyinfo/" + accountCode,
            type: "get",
            data: {}
        }).done(function (data) {
            refreshingForm = true;
            currentDCCode = data.dcCode;
            updatePayMethods(data.paymentMethods);
            updateForm();
            refreshingForm = false;
        });
    }

    checkAccount();
});