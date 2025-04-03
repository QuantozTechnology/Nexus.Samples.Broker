$(function () {

    var accountValid = false;
    var minBTC = 0.0;
    var maxBTC = 0.0;
    var updateSellForm;
    var refreshingForm = false;

    function checkAccount() {
        var accountCode = $('input[name=AccountCode]').val();

        if (accountCode.length < 8) {
            return;
        }

        $.ajax({
            dataType: "json",
            type: "GET",
            url: '/api/ajax/checksellinfo/' + accountCode,
            headers: {}
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
                            window.location.href = "/sell/" + supportedCrypto.route + "?id=" + getAccountCode();
                            redirecting = true;
                        } else {
                            $('#wrong-coin').prop('visible', 'block').show();
                            submit.prop('disabled', true);
                            $('input[name=AccountCode]').removeProp('disabled').removeAttr('disabled');
                            accountValid = false;
                            return;
                        }
                    }
                });
            }
            else {
                updateAccountCode(data.accountValid, data.isBusiness, data.trustLevel);

                if (data.accountValid) {
                    $('input[name=CryptoAmount]').focus();

                    setAmountRanges(data.minBtcAmount, data.maxBtcAmount);
                }

                $('#CryptoAmount').val(data.minBtcAmount);
                updatePrices();
            }
        });
    }

    var redirecting = false;

    /// Update account input field according to web response
    function updateAccountCode(valid, business, trustLevel) {
        // true/false
        var submit = $('.btn.btn-send');
        var updateprices = false;

        if (valid) {
            $('input[name=AccountCode]').prop('readonly', 'readonly');
            $('#wrong-coin').hide();
            $('#non-active').hide();
            $('#new-account').hide();
            $('#new-personal').hide();
            $('#trusted-personal').hide();
            $('#verified-personal').hide();
            $('#new-business').hide();
            $('#verified-business').hide();
            $('#business-limited').hide();
            $('#personal-limited').hide();
            $('#new-business-verification').hide();
            $('#trusted-accounts-verification').hide();

            if (business) {
                if (trustLevel === "IDENTIFIED") {
                    $('#verified-business').show();
                    $('#CryptoAmount').removeProp('disabled').removeAttr('disabled');
                    updateprices = true;
                    //submit.removeProp('disabled').removeAttr('disabled');
                }
                else if (type == "NEW") {
                    $('#new-business').show();
                    submit.prop('disabled', true);
                }
            }
            else {
                if (trustLevel === "IDENTIFIED") {
                    $('#verified-personal').show();
                    $('#CryptoAmount').removeProp('disabled').removeAttr('disabled');
                    updateprices = true;
                }
                else if (type == "TRUSTED") {
                    $('#trusted-personal').show();
                    $('#trusted-accounts-verification').show();
                    $('#CryptoAmount').removeProp('disabled').removeAttr('disabled');
                    updateprices = true;
                }
                else if (type == "NEW") {
                    $('#new-personal').show();
                    $('#CryptoAmount').removeProp('disabled').removeAttr('disabled');
                    updateprices = true;
                }
            }

            accountValid = true;
            submit.prop('disabled', false);

            updateSellForm = setInterval(function () { refreshFormData(); }, 60000);
            if (updateprices) {
                updatePrices();
            }
        }
        else {
            $('#non-active').prop('visible', 'block').show();
            submit.prop('disabled', false);
            $('input[name=AccountCode]').removeProp('disabled').removeAttr('disabled');
        }
    }

    function setAmountRanges(min, max) {
        var content = "";
        minBTC = min;
        maxBTC = max;
        if (max == 0) {
            content = "day or month limit reached";
        }
        else {
            content = "min: " + minBTC + " max: " + maxBTC;
        }

        $("#btcAmountLimit").html(content);
        $("#btcAmountLimit").show();
    }

    $('#CryptoAmount').on("input keyup change", function () { updatePrices(); });

    function updatePrices() {
        $("#btcAmountLimit").show();
        var BTC = $('#CryptoAmount').val();
        if (isNaN(BTC) || (BTC <= 0.0) || (BTC < minBTC) || (BTC > maxBTC)) {
            var v = 0.0;
            $("#btcAmountLimit").css('color', 'red');
            $("#CryptoAmount").css('border-color', 'red');
            $('.btn.btn-send').prop('disabled', true);
            $('#EuroAmountBeforeFee').text(v.toFixed(2));
            $('#BTCSellPrice').text('');
            $('#EuroBankFee').text(v.toFixed(2));
            $('#EuroServiceFee').text(v.toFixed(2));
            $('#EuroAmountAfterFee').text(v.toFixed(2));
            return;
        }

        $("#CryptoAmount").css('border-color', '#d4d7de');
        $('.btn.btn-send').prop('disabled', false);

        $("#btcAmountLimit").css('color', '#02aa45');
        var postData = {
            AccountCode: getAccountCode(),
            BtcAmount: BTC,
            Currency: 'EUR',
            CryptoCode: cryptoCode
        };

        $.ajax({
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            type: "POST",
            url: '/api/ajax/simulatesell/',
            data: JSON.stringify(postData),
            headers: {}
        }).done(function (data) {
            $('#EuroAmountBeforeFee').text(data.valueInFiatBeforeFees.toFixed(2));
            $('#EuroBankFee').text(data.bankFee.toFixed(2));
            $('#EuroServiceFee').text(data.serviceFee.toFixed(2));
            $('#EuroAmountAfterFee').text(data.valueInFiatAfterFees.toFixed(2));
            $('.currency-code').text(data.currency);

            if (data.valueInFiatAfterFees > 0) {
                $('.btn.btn-send').prop('disabled', false);
            }
            else {
                $('.btn.btn-send').prop('disabled', true);
            }
        });
    }

    $('input[name=AccountCode]').on("input keyup change", function () {
        $('input[name=AccountCode]').val(getAccountCode());
        checkAccount();
    });

    function getAccountCode() {
        return $('input[name=AccountCode]').val().trim();
    }

    /// pressing enter while typing accountcode will click CheckAccount button
    $('input[name=AccountCode]').on('keypress', function (event) {
        if (event.which === '13') {
            event.preventDefault();
        }
    });

    function refreshFormData() {
        var accountCode = getAccountCode();
        $.ajax({
            dataType: "json",
            type: "GET",
            url: '/api/ajax/checksellinfo/' + accountCode,
            headers: {}
        }).done(function (data) {
            refreshingForm = true;
            updatePrices();
            refreshingForm = false;
        });
    }

    checkAccount();
});