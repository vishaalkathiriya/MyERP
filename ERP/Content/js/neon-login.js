


(function ($, window, undefined) {
    "use strict";


    $(document).ready(function () {

        neonLogin.$container = $("#form_login");

        //neonLogin.$container.validate({
        //    rules: {
        //        username: { required: true },
        //        password: { required: true },
        //    },
        //        $(element).closest('.input-group').addClass('validate-has-error');
        //    },
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


        //neonLogin.$login_progressbar_indicator = $(".login-progressbar-indicator h3");
        //neonLogin.$login_progressbar = neonLogin.$body.find(".login-progressbar div");
        //neonLogin.$login_progressbar_indicator.html('0%');

        if (neonLogin.$body.hasClass('login-form-fall')) {
            var focus_set = false;
            setTimeout(function () {
                neonLogin.$body.addClass('login-form-fall-init')
                    if (!focus_set) {
                        neonLogin.$container.find('input:first').focus();
                        focus_set = true;
                    }
                }, 550);
            }, 0);
        }
            neonLogin.$container.find('input:first').focus();
        }
            var $this = $(el),
                $this.focus();
            });

            $this.on({
                focus: function () {
                    $group.addClass('focused');
                },
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
        //            pct: parseInt(pct, 10),
        //                neonLogin.$login_progressbar_indicator.html(o.pct + '%');
        //            },
        //            onComplete: callback
        //        });
        //    }
        //});

        
    });

})(jQuery, window);