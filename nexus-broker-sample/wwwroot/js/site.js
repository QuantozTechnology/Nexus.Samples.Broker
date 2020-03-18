$(document).ready(function () {
    $(".menu .menu__item--dropdown .menu__link").click(function (e) {
        e.preventDefault();
        toggleDropdown($(this));
    });

    $(".header__toggle").click(function (e) {
        e.preventDefault();
        toggleMobile();
    });

    $(document).mouseup(function (e) {
        var dropdown = $(".menu .menu__item--dropdown");
        if (!dropdown.is(e.target) && dropdown.has(e.target).length === 0) {
            toggleDropdown();
        }
    });

    toggleHeader();
    $(".header").addClass("header--init");

    setTimeout(function() {
        $(window).scroll(function () {
            if ($(window).scrollTop() >= 80) {
                $(".header").addClass("header--scroll");
            } else {
                $(".header").removeClass("header--scroll");
            }
        });
    }, 1000);

    function toggleDropdown(dropdown) {
        $(".menu .menu__item--dropdown .menu__link").parent().removeClass("menu__item--dropdown-active");
        $(".menu .menu__item--dropdown .menu__link").parent().find(".dropdown").removeClass("dropdown--toggle");

        if (dropdown) {
            dropdown.parent().toggleClass("menu__item--dropdown-active");
            dropdown.parent().find(".dropdown").toggleClass("dropdown--toggle");
        }
    }

    function toggleMobile() {
        $(".header__menu").css("top", $(".header").outerHeight());
        $(".header__menu .menu").css("padding-right", ($(window).width() - ($(".header__toggle").offset().left + $(".header__toggle").outerWidth())));

        $(".header").toggleClass("header--toggle");
    }

    function toggleHeader() {
        $("body").css({
            "padding-top": $(".header").outerHeight()
        });

        if ($(window).scrollTop() >= 80) {
            $(".header").addClass("header--scroll");
        } else {
            $(".header").removeClass("header--scroll");
        }
    }

    $('select').selectric({
        labelBuilder: function (currItem) {
            return (currItem.value.length ? '<span class="ico ico-' + currItem.value + '"></span>' : '') + currItem.text;
        },

        optionsItemBuilder: function (currItem) {
            return (currItem.value.length ? '<span class="ico ico-' + currItem.value + '"></span>' : '') + currItem.text;
        }
    });

    $(".select.select--payments select").change(function() {
        var language = $(this).children("option:selected").val();

        $(this).closest(".select.select--payments").find(".select__payment").removeClass("select__payment--active");
        $(this).closest(".select.select--payments").find(".select__payment").each(function(index) {
            if ($(this).attr("data-lang").indexOf(language) > -1) {
                $(this).addClass("select__payment--active");
            }
        });
    });

    $(".select.select--payments select").trigger("change");
});
