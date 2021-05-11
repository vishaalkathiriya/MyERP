var neonLogin = neonLogin || {};;


(function ($, window, undefined) {
    "use strict";


    $(document).ready(function () {

        neonLogin.$container = $("#form_login");

        //neonLogin.$container.validate({
        //    rules: {
        //        username: { required: true },
        //        password: { required: true },
        //    },        //    highlight: function (element) {
        //        $(element).closest('.input-group').addClass('validate-has-error');
        //    },        //    unhighlight: function (element) {
        //        $(element).closest('.input-group').removeClass('validate-has-error');
        //    },
        //    submitHandler: function (ev) {
        //        $(".login-page").addClass('logging-in');




        //        //setTimeout(function () {
        //        //    var random_pct = 25 + Math.round(Math.random() * 30);
        //        //    neonLogin.setPercentage(random_pct, function () {
        //        //        setTimeout(function () {
        //        //            neonLogin.setPercentage(100, function () {
        //        //                setTimeout("window.location.href = '../../'", 600);
        //        //            }, 2);
        //        //        }, 820);
        //        //    });
        //        //}, 650);

        //    }
        //});

        neonLogin.$body = $(".login-page");
        //neonLogin.$login_progressbar_indicator = $(".login-progressbar-indicator h3");
        //neonLogin.$login_progressbar = neonLogin.$body.find(".login-progressbar div");
        //neonLogin.$login_progressbar_indicator.html('0%');

        if (neonLogin.$body.hasClass('login-form-fall')) {
            var focus_set = false;
            setTimeout(function () {
                neonLogin.$body.addClass('login-form-fall-init')                setTimeout(function () {
                    if (!focus_set) {
                        neonLogin.$container.find('input:first').focus();
                        focus_set = true;
                    }
                }, 550);
            }, 0);
        }        else {
            neonLogin.$container.find('input:first').focus();
        }        neonLogin.$container.find('.form-control').each(function (i, el) {
            var $this = $(el),                $group = $this.closest('.input-group');            $this.prev('.input-group-addon').click(function () {
                $this.focus();
            });

            $this.on({
                focus: function () {
                    $group.addClass('focused');
                },                blur: function () {
                    $group.removeClass('focused');
                }
            });

        });


        //$.extend(neonLogin, {
        //    setPercentage: function (pct, callback) {
        //        pct = parseInt(pct / 100 * 100, 10) + '%';
        //        neonLogin.$login_progressbar_indicator.html(pct);
        //        neonLogin.$login_progressbar.width(pct);
        //        var o = { pct: parseInt(neonLogin.$login_progressbar.width() / neonLogin.$login_progressbar.parent().width() * 100, 10) };
        //        TweenMax.to(o, .7, {
        //            pct: parseInt(pct, 10),        //            roundProps: ["pct"],        //            ease: Sine.easeOut,        //            onUpdate: function () {
        //                neonLogin.$login_progressbar_indicator.html(o.pct + '%');
        //            },
        //            onComplete: callback
        //        });
        //    }
        //});

        
    });

})(jQuery, window);