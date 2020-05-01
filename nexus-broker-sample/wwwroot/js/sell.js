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
        headers: {
            //'RequestVerificationToken': tokenHeaderValue
        }
    }).done(function (data) {
        updateAccountCode(data, data.accountValid, data.isBusiness, data.accountType, data.highRisk, (data.firstBuyStatus == 0));

        if (data.accountValid) {
            $('input[name=BTCstr]').focus();

            setAmountRanges(data.minBtcAmount, data.maxBtcAmount, (data.highRisk || (data.firstBuyStatus > 0)));
            updateSellActivateComment(data.firstBuyStatus, data.HighRisk, data.isBusiness);
        }

        $('#BTCstr').val(data.minBtcAmount);
    });
}

var redirecting = false;

/// Update account input field according to web response
function updateAccountCode(data, valid, business, type, highrisk, hasfirstbuy) {
    // true/false
    var submit = $('.btn.btn-send');
    var updateprices = false;


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
                window.location.href = "/sell/" + crypto + "?id=" + getAccountCode();
                redirecting = true;
            } else {
                $('#wrong-coin').prop('visible', 'block').show();
                submit.prop('disabled', true);
                $('input[name=AccountCode]').removeProp('disabled').removeAttr('disabled');
                accountValid = false;
                return;
            }
        }
    }

    if (valid == true) {
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

        if (business == true) {
            if (type == "Identified") {
                $('#verified-business').show();
                if (highrisk == true) {
                    $('#business-limited').show();
                    submit.prop('disabled', true);
                }
                else {
                    $('#BTCstr').removeProp('disabled').removeAttr('disabled');
                    updateprices = true;
                    //submit.removeProp('disabled').removeAttr('disabled');
                }
            }
            else if (type == "New") {
                $('#new-business').show();
                if (hasfirstbuy == true) {
                    $('#new-business-verification').show();
                }
                submit.prop('disabled', true);
            }
        }
        else {
            if (type == "Identified") {
                $('#verified-personal').show();
                if (highrisk == true) {
                    $('#personal-limited').show();
                    submit.prop('disabled', true);
                }
                else {
                    $('#BTCstr').removeProp('disabled').removeAttr('disabled');
                    updateprices = true;
                    //submit.removeProp('disabled').removeAttr('disabled');
                }
            }
            else if (type == "Trusted") {
                $('#trusted-personal').show();
                if (highrisk == true) {
                    $('#personal-limited').show();
                    submit.prop('disabled', true);
                }
                else {
                    $('#trusted-accounts-verification').show();
                    $('#BTCstr').removeProp('disabled').removeAttr('disabled');
                    updateprices = true;
                    //submit.removeProp('disabled').removeAttr('disabled');
                }
            }
            else if (type == "New") {
                $('#new-personal').show();
                if (highrisk == true) {
                    $('#personal-limited').show();
                    submit.prop('disabled', true);
                }
                else {
                    $('#BTCstr').removeProp('disabled').removeAttr('disabled');
                    updateprices = true;
                    //submit.removeProp('disabled').removeAttr('disabled');
                }
            }
        }

        accountValid = true;
        submit.prop('disabled', false);

        updateSellForm = setInterval(function () { refreshFormData(); }, 60000);
        if (updateprices == true) {
            updatePrices();
        }
    }
    else {
        $('#non-active').prop('visible', 'block').show();
        submit.prop('disabled', false);
        $('input[name=AccountCode]').removeProp('disabled').removeAttr('disabled');
    }
}

function setAmountRanges(min, max, notallowed) {
    var content = "";
    if (notallowed == true) {
        minBTC = 0.0;
        maxBTC = 0.0;
    }
    else {
        minBTC = min;
        maxBTC = max;
        if (max == 0) {
            content = "day or month limit reached";
        }
        else {
            content = "min: " + minBTC + " max: " + maxBTC;
        }
    }
    $("#btcAmountLimit").html(content);
}

function updateSellActivateComment(type, highrisk, isbusiness) {
    if (type !== undefined && type !== null && isbusiness == false && highrisk == false) {
        if (type == 1) $('#sell-notice-needsuccessfulbuytransaction-beforefirstsell').show();
        if (type == 2) $('#sell-notice-successfulbuytransaction-pending').show();
        if (type == 3) $('#sell-notice-need-fotoid').show();
    }
}

$('#BTCstr').on("input keyup change", function () { updatePrices(); });

function updatePrices() {
    $("#btcAmountLimit").show();
    var BTCstr = $('#BTCstr').val();
    var BTC = parseFloat('0' + BTCstr);
    if (isNaN(BTCstr) || (BTC <= 0.0) || (BTC < minBTC) || (BTC > maxBTC)) {
        var v = 0.0;
        $("#btcAmountLimit").css('color', 'red');
        $("#BTCstr").css('border-color', 'red');
        $('.btn.btn-send').prop('disabled', true);
        $('#EuroAmountBeforeFee').text(v.toFixed(2));
        $('#BTCSellPrice').text('');
        $('#EuroBankFee').text(v.toFixed(2));
        $('#EuroServiceFee').text(v.toFixed(2));
        $('#EuroAmountAfterFee').text(v.toFixed(2));
        return;
    }

    $("#BTCstr").css('border-color', '#d4d7de');
    $('.btn.btn-send').prop('disabled', false);

    $("#btcAmountLimit").css('color', '#02aa45');
    var postData = {
        AccountCode: getAccountCode(),
        BtcAmount: BTC,
        Currency: '',
        CryptoCode: cryptoCode
    };

    $.ajax({
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        type: "POST",
        url: '/api/ajax/eurovalue/',
        data: JSON.stringify(postData),
        headers: {
            //'RequestVerificationToken': tokenHeaderValue
        }
    }).done(function (data) {
        $('#EuroAmountBeforeFee').text(data.valueInFiatBeforeFees.toFixed(2));
        //$('#BTCSellPrice').text('(' + data.btcSellPriceBeforeFee.toFixed(2) + ' ' + data.currency + '/' + data.dcCode + ')');
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
    if (event.which == '13') {
        event.preventDefault();
    }
});

function refreshFormData() {
    var accountCode = getAccountCode();
    $.ajax({
        dataType: "json",
        type: "GET",
        url: '/api/ajax/checksellinfo/' + accountCode,
        headers: {
            //'RequestVerificationToken': tokenHeaderValue
        }
    }).done(function (data) {
        refreshingForm = true;
        updatePrices();
        refreshingForm = false;
    });
}