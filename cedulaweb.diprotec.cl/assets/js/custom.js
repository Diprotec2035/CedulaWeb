$(function () {
    'use strict'

    // ______________ Page Loading
    $("#global-loader").fadeOut("slow");

    // ______________ Card
    const DIV_CARD = 'div.card';

    // ______________ Function for remove card
    $(document).on('click', '[data-bs-toggle="card-remove"]', function (e) {
        let $card = $(this).closest(DIV_CARD);
        $card.remove();
        e.preventDefault();
        return false;
    });

    // ______________ Functions for collapsed card
    $(document).on('click', '[data-bs-toggle="card-collapse"]', function (e) {
        let $card = $(this).closest(DIV_CARD);
        $card.toggleClass('card-collapsed');
        e.preventDefault();
        return false;
    });


    // ______________ Card full screen
    $(document).on('click', '[data-bs-toggle="card-fullscreen"]', function (e) {
        let $card = $(this).closest(DIV_CARD);
        $card.toggleClass('card-fullscreen').removeClass('card-collapsed');
        e.preventDefault();
        return false;   
    });

    // ______________ COVER IMAGE
    $(".cover-image").each(function () {
        var attr = $(this).attr('data-bs-image-src');
        if (typeof attr !== typeof undefined && attr !== false) {
            $(this).css('background', 'url(' + attr + ') center center');
        }
    });

    // ______________Tooltip
    var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'))
    var tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl)
    })
    // ______________Popover
    var popoverTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="popover"]'))
    var popoverList = popoverTriggerList.map(function (popoverTriggerEl) {
        return new bootstrap.Popover(popoverTriggerEl)
    })
    // ______________Toast
    $(".toast").toast();
    // ______________ Toast
    var toastElList = [].slice.call(document.querySelectorAll('.toast'))
    var toastList = toastElList.map(function (toastEl) {
        return new bootstrap.Toast(toastEl)
    })


    // ______________Back-top-button
    $(window).on("scroll", function (e) {
        if ($(this).scrollTop() > 0) {
            $('#back-to-top').fadeIn('slow');
        } else {
            $('#back-to-top').fadeOut('slow');
        }
    });
    $(document).on("click", "#back-to-top", function (e) {
        $("html, body").animate({
            scrollTop: 0
        }, 0);
        return false;
    });

    // ______________Full screen
    $(document).on("click", ".fullscreen-button", function toggleFullScreen() {
        $('html').addClass('fullscreenie');
        if ((document.fullScreenElement !== undefined && document.fullScreenElement === null) || (document.msFullscreenElement !== undefined && document.msFullscreenElement === null) || (document.mozFullScreen !== undefined && !document.mozFullScreen) || (document.webkitIsFullScreen !== undefined && !document.webkitIsFullScreen)) {
            if (document.documentElement.requestFullScreen) {
                document.documentElement.requestFullScreen();
            } else if (document.documentElement.mozRequestFullScreen) {
                document.documentElement.mozRequestFullScreen();
            } else if (document.documentElement.webkitRequestFullScreen) {
                document.documentElement.webkitRequestFullScreen(Element.ALLOW_KEYBOARD_INPUT);
            } else if (document.documentElement.msRequestFullscreen) {
                document.documentElement.msRequestFullscreen();
            }
        } else {
            $('html').removeClass('fullscreenie');
            if (document.cancelFullScreen) {
                document.cancelFullScreen();
            } else if (document.mozCancelFullScreen) {
                document.mozCancelFullScreen();
            } else if (document.webkitCancelFullScreen) {
                document.webkitCancelFullScreen();
            } else if (document.msExitFullscreen) {
                document.msExitFullscreen();
            }
        }
    })

    // ______________Cover Image
    $(".cover-image").each(function () {
        var attr = $(this).attr('data-image-src');
        if (typeof attr !== typeof undefined && attr !== false) {
            $(this).css('background', 'url(' + attr + ') center center');
        }
    });


    // OFF-CANVAS STYLE
    $('.off-canvas').on('click', function () {
        $('body').addClass('overflow-y-scroll');
        $('body').addClass('pe-0');
    });


    function replay() {
        let replayButtom = document.querySelectorAll('.reply a')
        // Creating Div
        let Div = document.createElement('div')
        Div.setAttribute('class', "comment mt-4 d-grid")
        // creating textarea
        let textArea = document.createElement('textarea')
        textArea.setAttribute('class', "form-control")
        textArea.setAttribute('rows', "5")
        textArea.innerText = "Your Comment";
        // creating Cancel buttons
        let cancelButton = document.createElement('button');
        cancelButton.setAttribute('class', "btn btn-danger");
        cancelButton.innerText = "Cancel";

        let buttonDiv = document.createElement('div')
        buttonDiv.setAttribute('class', "btn-list ms-auto mt-2")

        // Creating submit button
        let submitButton = document.createElement('button');
        submitButton.setAttribute('class', "btn btn-success");
        submitButton.innerText = "Submit";

        // appending text are to div
        Div.append(textArea)
        Div.append(buttonDiv);
        buttonDiv.append(cancelButton);
        buttonDiv.append(submitButton);

        replayButtom.forEach((element, index) => {

            element.addEventListener('click', () => {
                let replay = $(element).parent()
                replay.append(Div)

                cancelButton.addEventListener('click', () => {
                    Div.remove()
                })
            })
        })


    }
    replay()

        
    // ______________ SWITCHER-toggle ______________//
    $('.layout-setting').on("click", function (e) {
		let html = document.querySelector('html');
		if (html.getAttribute('data-theme-color') === "dark") {
			html.setAttribute('data-theme-color', 'light');
			html.setAttribute('data-header-style', 'light');
			html.setAttribute('data-menu-style', 'light');
			$('#switchbtn-lightmenu').prop('checked', true);
			$('#switchbtn-lightheader').prop('checked', true);
            
            $('#switchbtn-light').prop('checked', true);
			localStorage.setItem("viboonlighttheme", true);
			localStorage.removeItem("Viboondarktheme");
			localStorage.removeItem("ViboonbgColor");
			localStorage.removeItem("Viboonheaderbg");
			localStorage.removeItem("Viboonbgwhite");
			localStorage.removeItem("Viboonmenubg");
			localStorage.removeItem("ViboontransparentBgColor");

			localStorage.setItem("ViboonHeader", 'light');
        	localStorage.setItem("ViboonMenu", 'light');

			checkOptions();

			// if (!document.body.classList.contains('auth-page')) {
			// 	let mainHeader = document.querySelector('.app-header');
			// 	mainHeader.style = "";
			// 	let appSidebar = document.querySelector('.app-sidebar');
			// 	appSidebar.style = "";
			// }
			document.querySelector('html').style = '';
			names();

		} 
		else {
			html.setAttribute('data-theme-color', 'dark');
			html.setAttribute('data-header-style', 'dark');
			html.setAttribute('data-menu-style', 'dark');
			$('#switchbtn-darkmenu').prop('checked', true);
			$('#switchbtn-darkheader').prop('checked', true);
			
            $('#switchbtn-dark').prop('checked', true);
			localStorage.setItem("Viboondarktheme", true);
			localStorage.removeItem("Viboonlighttheme");
			localStorage.removeItem("ViboonbgColor");
			localStorage.removeItem("Viboonheaderbg");
			localStorage.removeItem("Viboonbgwhite");
			localStorage.removeItem("Viboonmenubg");

			localStorage.setItem("ViboonHeader", 'dark');
        	localStorage.setItem("ViboonMenu", 'dark');
			
			checkOptions();

			// if (!document.body.classList.contains('auth-page')) {
			// 	let mainHeader = document.querySelector('.app-header');
			// 	mainHeader.style = "";
			// 	let appSidebar = document.querySelector('.app-sidebar');
			// 	appSidebar.style = "";
			// }
			document.querySelector('html').style = '';
			names();
		}
	});

}); 

