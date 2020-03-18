$(function () {
    function loadMarketPlaces() {
        $.ajax("/api/ajax/getprices", {
            type: "get",
            contentType: "application/json",
            dataType: "json",
            data: {
                currency: "EUR"
            },
            headers: {
                //'RequestVerificationToken': tokenHeaderValue
            }
        }).done(function (data) {
            for (var i = 0; i < data.prices.length; i++) {
                var item = data.prices[i];

                $($('.buyPrice')).each(function(index) {
                    if ($(this).attr('data-crypto') === item.dc) {
                        $(this).html(item.buyText.match(/[\d\,.]+/));
                    }
                });

                $($('.sellPrice')).each(function(index) {
                    if ($(this).attr('data-crypto') === item.dc) {
                        $(this).html(item.sellText.match(/[\d\,.]+/));
                    }
                });
            };
        });
    };

    $(document).ready(function () {
        loadMarketPlaces();
        var updatempricestimer = setInterval(function () { loadMarketPlaces(); }, 60000);
    });
});