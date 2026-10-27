"use strict";
let mainContent;
(function () {
    let html = document.querySelector('html');
    mainContent = document.querySelector('.main-content');

    // theme color picker
    const dynamicPrimaryLight = document.querySelectorAll('input.color-primary-light');
    const dynamicBackground = document.querySelectorAll('input.color-bg-transparent');
    dynamicPrimaryColor(dynamicPrimaryLight);
    dynamicBackgroundColor(dynamicBackground);

    localStorageBackup();


    //LTR 
    if (!localStorage.getItem("Viboonltr")) {
        // html.setAttribute("dir" , "ltr") // for ltr version 
    }
    //RTL 
    if (!localStorage.getItem("Viboonrtl")) {
        // html.setAttribute("dir" , "rtl") // for rtl version 
    }

    //Light Theme Style
    if (!localStorage.getItem("Viboonlighttheme")) {
        // html.setAttribute("data-theme-color" , "light") // for light theme 
    }

    //Dark Theme Style
    if (!localStorage.getItem("Viboondarktheme")) {
        // html.setAttribute("data-theme-color" , "dark") // for dark theme 
    }

    //Menu Layout
    if (!localStorage.getItem("Viboonlayout")) {
        // html.setAttribute("data-layout" , "vertical") // for Vertical layout 
        // html.setAttribute("data-layout" , "horizontal") // for horizontal layout 
    }

    //Menu Styles
    if (!localStorage.getItem("ViboonMenu")) {
        // html.setAttribute("data-menu-style" , "light") // for light menu style 
        // html.setAttribute("data-menu-style" , "dark") // for dark menu style 
        // html.setAttribute("data-menu-style" , "color") // for color menu style
        // html.setAttribute("data-menu-style" , "gradient") // for gradient menu style 
    }

    //Header Styles
    if (!localStorage.getItem("ViboonHeader")) {
        // html.setAttribute("data-header-style" , "light") // for light header style 
        // html.setAttribute("data-header-style" , "dark") // for dark header style 
        // html.setAttribute("data-header-style" , "color") // for color header style 
        // html.setAttribute("data-header-style" , "gradient") // for gradient header style 
    }

    //Default Layout Styles
    if (!localStorage.getItem("Viboonverticalstyles")) {
        // html.setAttribute("data-vertical-style" , "default") // for Vertical default style 
    }

    //Closed Layout Styles
    if (!localStorage.getItem("Viboonverticalstyles")) {
        // html.setAttribute("data-vertical-style" , "closed") // for Vertical closed style 
        // $('body').addClass('sidenav-toggled');
    }

    //IconText Layout Styles
    if (!localStorage.getItem("Viboonverticalstyles")) {
        // html.setAttribute("data-vertical-style" , "icontext") // for Vertical icontext style 
        // textLayoutFn();
    }

    //Overlay Layout Styles
    if (!localStorage.getItem("Viboonverticalstyles")) {
        // html.setAttribute("data-vertical-style" , "overlay") // for Vertical overlay style 
        // $('body').addClass('sidenav-toggled');
    }

    //Hover Submenu Layout Styles
    if (!localStorage.getItem("Viboonverticalstyles")) {
        // html.setAttribute("data-vertical-style" , "hover") // for Vertical hover style 
        // hoverLayoutFn();
    }

    //Hover Submenu1 Layout Styles
    if (!localStorage.getItem("Viboonverticalstyles")) {
        // html.setAttribute("data-vertical-style" , "hover1") // for Vertical hover1 style 
        // hoverLayoutFn();
    }

    //Double Menu Layout Styles
    if (!localStorage.getItem("Viboonverticalstyles")) {
        // html.setAttribute("data-vertical-style" , "doublemenu") // for Vertical doublemenu style 
        // doubleLayoutFn();
    }

    //Double Menu Tabs Layout Styles
    if (!localStorage.getItem("Viboonverticalstyles")) {
        // html.setAttribute("data-vertical-style" , "doublemenu-tabs") // for Vertical doublemenu-tabs style 
        // doubleLayoutFn();
    }

    //horizontalmenu Layout Styles
    if (!localStorage.getItem("Viboonlayout") === "horizontal" || localStorage.getItem("Viboonlayout") == null) {
        // html.setAttribute("data-hor-style" , "hor-click") // for horizontal click style 
        // html.setAttribute("data-hor-style" , "hor-hover") // for horizontal hover style 
    }

    //Fullwidth styles
    if (!localStorage.getItem("Viboonfullwidth")) {
        // html.setAttribute("data-width" , "fullwidth") // for fullwidth style
    }
    //Boxed styles
    if (!localStorage.getItem("Viboonboxed")) {
        // html.setAttribute("data-width" , "boxed") // for boxed style
    }

    //Fixed styles
    if (!localStorage.getItem("Viboonfixed")) {
        // html.setAttribute("data-position" , "fixed") // for fixed style
    }
    //Scrollabel styles
    if (!localStorage.getItem("Viboonscrollable")) {
        // html.setAttribute("data-position" , "scrollable") // for scrollable style
    }

    //Shadow styles
    if (!localStorage.getItem("Viboonshadow")) {
        // html.setAttribute("data-skins" , "shadow") // for shadow style
    }
    //No-Shadow styles
    if (!localStorage.getItem("Viboonnoshadow")) {
        // html.setAttribute("data-skins" , "no-shadow") // for no shadow style
    }

    /*RTL Start*/
    if (html.getAttribute('dir') === "rtl") {
        rtlFn();
    }
    /*RTL End*/

    /*Horizontal Start*/
    if (html.getAttribute('data-hor-style') === "hor-click") {
        horizontalClickFn();
    }
    /*Horizontal End*/

    /*Horizontal-Hover Start*/
    if (html.getAttribute('data-hor-style') === "hor-hover") {
        horizontalHoverFn();
    }
    /*Horizontal-Hover End*/

    if (document.querySelector(".sidebar-right1")) {
        switcherClick();
    }

    checkOptions();

})();

function switcherClick() {
    let ltrBtn, rtlBtn, verticalBtn, horiBtn, horiHoverBtn, lightBtn, darkBtn, boxedBtn, fullwidthBtn, scrollableBtn, fixedBtn, lightHeaderBtn, darkHeaderBtn, colorHeaderBtn, gradientHeaderBtn, lightMenuBtn, darkMenuBtn, colorMenuBtn, gradientMenuBtn, shadowBtn, NoshadowBtn, defaultBtn, closedBtn, iconTextBtn, hoversubBtn, hoversub1Btn, overlayBtn, doubleBtn, doubleTabsBtn, defaultlogoBtn, centerlogoBtn, resetBtn;
    let html = document.querySelector('html');
    lightBtn = document.querySelector('#switchbtn-light');
    darkBtn = document.querySelector('#switchbtn-dark');
    ltrBtn = document.querySelector('#switchbtn-ltr');
    rtlBtn = document.querySelector('#switchbtn-rtl');
    verticalBtn = document.querySelector('#switchbtn-vertical');
    horiBtn = document.querySelector('#switchbtn-horizontal');
    horiHoverBtn = document.querySelector('#switchbtn-horizontalHover');
    boxedBtn = document.querySelector('#switchbtn-boxed');
    fullwidthBtn = document.querySelector('#switchbtn-fullwidth');
    scrollableBtn = document.querySelector('#switchbtn-scrollable');
    fixedBtn = document.querySelector('#switchbtn-fixed');
    lightHeaderBtn = document.querySelector('#switchbtn-lightheader');
    darkHeaderBtn = document.querySelector('#switchbtn-darkheader');
    colorHeaderBtn = document.querySelector('#switchbtn-colorheader');
    gradientHeaderBtn = document.querySelector('#switchbtn-gradientheader');
    lightMenuBtn = document.querySelector('#switchbtn-lightmenu');
    darkMenuBtn = document.querySelector('#switchbtn-darkmenu');
    colorMenuBtn = document.querySelector('#switchbtn-colormenu');
    gradientMenuBtn = document.querySelector('#switchbtn-gradientmenu');
    shadowBtn = document.querySelector('#switchbtn-shadow');
    NoshadowBtn = document.querySelector('#switchbtn-noshadow');
    defaultBtn = document.querySelector('#switchbtn-defaultmenu');
    closedBtn = document.querySelector('#switchbtn-closed');
    iconTextBtn = document.querySelector('#switchbtn-text');
    hoversubBtn = document.querySelector('#switchbtn-hoversub');
    hoversub1Btn = document.querySelector('#switchbtn-hoversub1');
    overlayBtn = document.querySelector('#switchbtn-overlay');
    doubleBtn = document.querySelector('#switchbtn-doublemenu');
    doubleTabsBtn = document.querySelector('#switchbtn-doublemenu-tabs');
    defaultlogoBtn = document.querySelector('#switchbtn-defaultlogo');
    centerlogoBtn = document.querySelector('#switchbtn-centerlogo');
    resetBtn = document.querySelector('#resetbtn');

    /*Light Layout Start*/
    let lightThemeVar = lightBtn.addEventListener('click', () => {
        html.setAttribute('data-theme-color', 'light');
        html.setAttribute('data-header-style', 'light');
        html.setAttribute('data-menu-style', 'light');
        $('#switchbtn-lightmenu').prop('checked', true);
        $('#switchbtn-lightheader').prop('checked', true);
        document.querySelector('html').style.removeProperty('--white');

        localStorage.setItem("Viboonlighttheme", true);
        localStorage.removeItem("Viboondarktheme");
        localStorage.removeItem("ViboonbgColor");
        localStorage.removeItem("Viboonheaderbg");
        localStorage.removeItem("Viboonbgwhite");
        localStorage.removeItem("Viboonmenubg");
        localStorage.removeItem("ViboontransparentBgColor");

        localStorage.setItem("ViboonHeader", 'light');
        localStorage.setItem("ViboonMenu", 'light');
        localStorage.setItem("ViboonMenu", 'light');

        checkOptions();
        // const root = document.querySelector(':root');
        // root.style = "";
        names();

        if (!document.body.classList.contains('login-img')) {
            let mainHeader = document.querySelector('.main-header');
            mainHeader.style = "";
            let appSidebar = document.querySelector('.app-sidebar');
            appSidebar.style = "";
        }

    })
    /*Light Layout End*/

    /*Dark Layout Start*/
    let darkThemeVar = darkBtn.addEventListener('click', () => {
        html.setAttribute('data-theme-color', 'dark');
        html.setAttribute('data-header-style', 'dark');
        html.setAttribute('data-menu-style', 'dark');
        $('#switchbtn-darkmenu').prop('checked', false);
        $('#switchbtn-darkheader').prop('checked', false);

        localStorage.setItem("Viboondarktheme", true);
        localStorage.removeItem("Viboonlighttheme");
        localStorage.removeItem("ViboonbgColor");
        localStorage.removeItem("Viboonheaderbg");
        localStorage.removeItem("Viboonbgwhite");
        localStorage.removeItem("Viboonmenubg");
        localStorage.removeItem("ViboontransparentBgColor");
        
        document.querySelector('html').style.removeProperty('--primary-bg-rgb', localStorage.ViboontransparentBgColor);
        document.querySelector('html').style.removeProperty('--white', localStorage.ViboontransparentBgColor);


        localStorage.setItem("ViboonHeader", 'dark');
        localStorage.setItem("ViboonMenu", 'dark');

        checkOptions();

        // const root = document.querySelector(':root');
        // root.style = "";
        names();

        if (!document.body.classList.contains('login-img')) {
            let mainHeader = document.querySelector('.main-header');
            mainHeader.style = "";
            let appSidebar = document.querySelector('.app-sidebar');
            appSidebar.style = "";
        }

    });
    /*Dark Layout End*/

    /*Light Menu Start*/
    let lightMenuVar = lightMenuBtn?.addEventListener('click', () => {
        html.setAttribute('data-menu-style', 'light');
        let appSidebar = document.querySelector('.app-sidebar');
        appSidebar.style = "";
        localStorage.setItem("ViboonMenu", 'light');
    });
    /*Light Menu End*/

    /*Color Menu Start*/
    let colorMenuVar = colorMenuBtn?.addEventListener('click', () => {
        html.setAttribute('data-menu-style', 'color');
        let appSidebar = document.querySelector('.app-sidebar');
        appSidebar.style = "";
        localStorage.setItem("ViboonMenu", 'color');
    });
    /*Color Menu End*/

    /*Dark Menu Start*/
    let darkMenuVar = darkMenuBtn?.addEventListener('click', () => {
        html.setAttribute('data-menu-style', 'dark');
        let appSidebar = document.querySelector('.app-sidebar');
        appSidebar.style = "";
        localStorage.setItem("ViboonMenu", 'dark');
    });
    /*Dark Menu End*/

    /*Gradient Menu Start*/
    let gradientMenuVar = gradientMenuBtn?.addEventListener('click', () => {
        html.setAttribute('data-menu-style', 'gradient');
        let appSidebar = document.querySelector('.app-sidebar');
        appSidebar.style = "";
        localStorage.setItem("ViboonMenu", 'gradient');
    });
    /*Gradient Menu End*/

    /*Light Header Start*/
    let lightHeaderVar = lightHeaderBtn?.addEventListener('click', () => {
        html.setAttribute('data-header-style', 'light');
        let mainHeader = document.querySelector('.main-header');
        mainHeader.style = "";
        localStorage.setItem("ViboonHeader", 'light');
    });
    /*Light Header End*/

    /*Color Header Start*/
    let colorHeaderVar = colorHeaderBtn?.addEventListener('click', () => {
        html.setAttribute('data-header-style', 'color');
        let mainHeader = document.querySelector('.main-header');
        mainHeader.style = "";
        localStorage.setItem("ViboonHeader", 'color');
    });
    /*Color Header End*/

    /*Dark Header Start*/
    let darkHeaderVar = darkHeaderBtn?.addEventListener('click', () => {
        html.setAttribute('data-header-style', 'dark');
        let mainHeader = document.querySelector('.main-header');
        mainHeader.style = "";
        localStorage.setItem("ViboonHeader", 'dark');
    });
    /*Dark Header End*/

    /*Gradient Header Start*/
    let gradientHeaderVar = gradientHeaderBtn?.addEventListener('click', () => {
        html.setAttribute('data-header-style', 'gradient');
        let mainHeader = document.querySelector('.main-header');
        mainHeader.style = "";
        localStorage.setItem("ViboonHeader", 'gradient');
    });
    /*Gradient Header End*/

    /*Full Width Layout Start*/
    let fullwidthVar = fullwidthBtn?.addEventListener('click', () => {
        html.setAttribute('data-width', 'fullwidth');
        if (html.getAttribute('data-layout') === "horizontal") {
            checkHoriMenu();
        }
        localStorage.setItem("Viboonfullwidth", true);
        localStorage.removeItem("Viboonboxed");
    });
    /*Full Width Layout End*/

    /*Boxed Layout Start*/
    let boxedVar = boxedBtn?.addEventListener('click', () => {
        html.setAttribute('data-width', 'boxed');
        if (html.getAttribute('data-layout') === "horizontal") {
            checkHoriMenu();
        }
        localStorage.setItem("Viboonboxed", true);
        localStorage.removeItem("Viboonfullwidth");
    });
    /*Boxed Layout End*/

    /*Shadow Layout Start*/
    let shadowVar = shadowBtn?.addEventListener('click', () => {
        html.setAttribute('data-skins', 'shadow');
        localStorage.setItem("Viboonshadow", true);
        localStorage.removeItem("Viboonnoshadow");
    });
    /*Shadow Layout End*/

    /*No Shadow Layout Start*/
    let noShadowVar = NoshadowBtn?.addEventListener('click', () => {
        html.setAttribute('data-skins', 'no-shadow');
        localStorage.setItem("Viboonnoshadow", true);
        localStorage.removeItem("Viboonshadow");
    });
    /*No Shadow Layout End*/

    /*Header-Position Styles Start*/
    let fixedVar = fixedBtn?.addEventListener('click', () => {
        html.setAttribute('data-position', 'fixed');
        localStorage.setItem("Viboonfixed", true);
        localStorage.removeItem("Viboonscrollable");
    });

    let scrollableVar = scrollableBtn?.addEventListener('click', () => {
        html.setAttribute('data-position', 'scrollable');
        localStorage.setItem("Viboonscrollable", true);
        localStorage.removeItem("Viboonfixed");
    });
    /*Header-Position Styles End*/

    /*Default Sidemenu Start*/
    let defaultVar = defaultBtn?.addEventListener('click', () => {
        html.setAttribute('data-vertical-style', 'default');
        document.body.classList.remove('sidenav-toggled');
        localStorage.removeItem("Viboonverticalstyles");

        hovermenu();
    });
    /*Default Sidemenu End*/

    /*Closed Sidemenu Start*/
    let closedVar = closedBtn?.addEventListener('click', () => {
        html.setAttribute('data-vertical-style', 'closed');
        localStorage.setItem("Viboonverticalstyles", 'closed');

        hoverLayoutFn();
    });
    /*Closed Sidemenu End*/

    /*Hover Submenu Start*/
    let hoverSubVar = hoversubBtn?.addEventListener('click', () => {
        html.setAttribute('data-vertical-style', 'hover');
        localStorage.setItem("Viboonverticalstyles", 'hover');

        hoverLayoutFn();
    });
    /*Hover Submenu End*/

    /*Hover Submenu 1 Start*/
    let hoverSub1Var = hoversub1Btn?.addEventListener('click', () => {
        html.setAttribute('data-vertical-style', 'hover1');
        localStorage.setItem("Viboonverticalstyles", 'hover1');

        hoverLayoutFn();
    });
    /*Hover Submenu 1 End*/

    /*Icon Text Sidemenu Start*/
    let iconTextVar = iconTextBtn?.addEventListener('click', () => {
        html.setAttribute('data-vertical-style', 'icontext');
        localStorage.setItem("Viboonverticalstyles", 'icontext');

        textLayoutFn();
    });
    /*Icon Text Sidemenu End*/

    /*Icon Overlay Sidemenu Start*/
    let overlayVar = overlayBtn?.addEventListener('click', () => {
        html.setAttribute('data-vertical-style', 'overlay');
        localStorage.setItem("Viboonverticalstyles", 'overlay');

        hoverLayoutFn();
    });
    /*Icon Overlay Sidemenu End*/

    /*Double Menu Sidemenu Start*/
    let doubleVar = doubleBtn?.addEventListener('click', () => {
        html.setAttribute('data-vertical-style', 'doublemenu');
        localStorage.setItem("Viboonverticalstyles", 'doublemenu');

        doubleLayoutFn();
    });
    /*Double Menu Sidemenu End*/

    /*Double Menu Sidemenu Start*/
    let doubleTabsVar = doubleTabsBtn?.addEventListener('click', () => {
        html.setAttribute('data-vertical-style', 'doublemenu-tabs');
        localStorage.setItem("Viboonverticalstyles", 'doublemenu-tabs');

        doubleLayoutFn();
    });
    /*Double Menu Sidemenu End*/

    /* Sidemenu start*/
    let verticalVar = verticalBtn?.addEventListener('click', () => {
        // local storage
        localStorage.removeItem("Viboonlayout");
        localStorage.setItem("Viboonverticalstyles", 'default');

        verticalFn();
    });
    /* Sidemenu end*/

    /* horizontal click start*/
    let horiVar = horiBtn?.addEventListener('click', () => {

        //    local storage 
        localStorage.setItem("Viboonlayout", 'horizontal');
        localStorage.removeItem("Viboonverticalstyles");

        horizontalClickFn();
    });
    /* horizontal click end*/

    /* horizontal hover start*/
    let horiHoverVar = horiHoverBtn?.addEventListener('click', () => {

        //    local storage 
        localStorage.setItem("Viboonlayout", 'horizontalhover');
        localStorage.removeItem("Viboonverticalstyles");

        horizontalHoverFn();
    });
    /* horizontal hover end*/
    /* rtl start*/
    let rtlVar = rtlBtn?.addEventListener('click', () => {
        localStorage.setItem("Viboonrtl", true);
        localStorage.removeItem("Viboonltr");
        rtlFn();
    });
    /* rtl end*/
    /* ltr start*/
    let ltrVar = ltrBtn?.addEventListener('click', () => {
        //    local storage 
        localStorage.setItem("Viboonltr", true);
        localStorage.removeItem("Viboonrtl");

        ltrFn();
    });
    /* ltr end*/


    /*Horizontal Logo Position Start*/
    let defaultlogoVar = defaultlogoBtn?.addEventListener('click', () => {
        html.setAttribute('data-logo', 'defaultlogo');
        localStorage.setItem("Viboondefaultlogo", true);
        localStorage.removeItem("Vibooncenterlogo");
    });

    let centerlogoVar = centerlogoBtn?.addEventListener('click', () => {
        html.setAttribute('data-logo', 'centerlogo');
        localStorage.setItem("Vibooncenterlogo", true);
        localStorage.removeItem("Viboondefaultlogo");
    });
    /*Horizontal Logo Position End*/
}

function ltrFn() {
    let html = document.querySelector('html');
    html.setAttribute("dir", "ltr");
    let select2Cont = document.querySelectorAll(".select2-container")
    select2Cont.forEach(e => e.setAttribute("dir", "ltr"))
    document.querySelector("#style")?.setAttribute("href", "../assets/plugins/bootstrap/css/bootstrap.min.css");
    var carousel = $('.owl-carousel');
    $.each(carousel, function (index, element) {
        // element == this
        var carouselData = $(element).data('owl.carousel');
        carouselData.settings.rtl = false; //don't know if both are necessary
        carouselData.options.rtl = false;
        $(element).trigger('refresh.owl.carousel');
    });
    if (html.getAttribute('data-layout') === "horizontal") {
        checkHoriMenu();
    }
    //
    checkOptions();
}

function checkHoriMenu(){}

function rtlFn() {
    let html = document.querySelector('html');
    html.setAttribute("dir", "rtl");
    let select2Cont = document.querySelectorAll(".select2-container")
    select2Cont.forEach(e => e.setAttribute("dir", "rtl"))
    document.querySelector("#style")?.setAttribute("href", "../assets/plugins/bootstrap/css/bootstrap.rtl.min.css");
    var carousel = $('.owl-carousel');
    $.each(carousel, function (index, element) {
        // element == this
        var carouselData = $(element).data('owl.carousel');
        carouselData.settings.rtl = true; //don't know if both are necessary
        carouselData.options.rtl = true;
        $(element).trigger('refresh.owl.carousel');
    });
    if (html.getAttribute('data-layout') === "horizontal"  && document.querySelector("login-img")) {
        checkHoriMenu();
    }
    //
    checkOptions();
}

function verticalFn() {
    $('#switchbtn-vertical').prop('checked', true);
    let html = document.querySelector('html');
    html.setAttribute('data-layout', 'vertical');
    html.setAttribute('data-vertical-style', 'default');
    html.removeAttribute('data-hor-style');
    if (!document.body.classList.contains('login-img')) {
        document.body.classList.add('sidebar-mini');
        document.querySelector(".side-content").classList.add("main-content");
        let mainContainer = document.querySelectorAll(".main-container")
        mainContainer.forEach(e => e.classList.add("container-fluid"))
        mainContainer.forEach(e => e.classList.remove("container"))
        document.querySelector(".main-content").classList.remove("horizontal-content");
        document.querySelector(".main-header").classList.remove("hor-header");
        document.querySelector(".app-sidebar").classList.remove("horizontal-main");
        document.querySelector(".main-sidemenu").classList.remove("container");
        document.querySelector('#slide-left').classList.remove('d-none');
        document.querySelector('#slide-right').classList.remove('d-none');
        if (html.getAttribute('data-layout') === "horizontal") {
            checkHoriMenu();
        }
        responsive();
        menuClick();
        ActiveSubmenu()
        mainContent.removeEventListener('click', slideClick);
        //
        checkOptions();
    }
}

function horizontalClickFn() {
    $('#switchbtn-horizontal').prop('checked', true);
    let html = document.querySelector('html');
    html.setAttribute('data-layout', 'horizontal');
    html.setAttribute('data-hor-style', 'hor-click');
    html.removeAttribute('data-vertical-style');
    if (!document.body.classList.contains('login-img')) {

        ActiveSubmenu();
        document.querySelector(".main-content")?.classList.add("horizontal-content");
        document.querySelector(".side-content")?.classList.remove("main-content");
        let mainContainer = document.querySelectorAll(".main-container")
        mainContainer.forEach(e => e.classList.add("container"))
        mainContainer.forEach(e => e.classList.remove("container-fluid"))
        document.querySelector(".main-header").classList.add("hor-header");
        document.querySelector(".app-sidebar").classList.add("horizontal-main");
        document.querySelector(".main-sidemenu").classList.add("container");

        document.querySelector(".side-content").classList.remove("main-content");
        document.body.classList.remove('sidebar-mini');
        document.body.classList.remove('sidenav-toggled');
        responsive();
        menuClick();
        mainContent.addEventListener('click', slideClick);
        setTimeout(() => {
            if (window.innerWidth >= 992) {
                slideClick()
            }
            checkHoriMenu();
        }, 800)
        //
        checkOptions();
    }
}

function horizontalHoverFn() {
    $('#switchbtn-horizontalHover').prop('checked', true);
    let html = document.querySelector('html');
    html.setAttribute('data-layout', 'horizontal');
    html.setAttribute('data-hor-style', 'hor-hover');
    html.removeAttribute('data-vertical-style');
    let li = document.querySelectorAll('.side-menu li')

    if (!document.body.classList.contains('login-img')) {

        document.querySelector(".main-content")?.classList.add("horizontal-content");
        document.querySelector(".side-content")?.classList.remove("main-content");
        let mainContainer = document.querySelectorAll(".main-container")
        mainContainer.forEach(e => e.classList.add("container"))
        mainContainer.forEach(e => e.classList.remove("container-fluid"))
        document.querySelector(".main-header").classList.add("hor-header");
        document.querySelector(".app-sidebar").classList.add("horizontal-main")
        document.querySelector(".main-sidemenu").classList.add("container")
        document.body.classList.remove('sidebar-mini');
        document.body.classList.remove('sidenav-toggled');
        responsive();
        menuClick();
        mainContent.removeEventListener('click', slideClick);
        //
        setTimeout(() => {
            if (window.innerWidth >= 992) {
                slideClick()
            }
            checkHoriMenu();
        }, 500)
        checkOptions();
    }
}

function resetData() {
    let html = document.querySelector('html');
    $('#switchbtn-ltr').prop('checked', true);
    $('#switchbtn-light').prop('checked', true);
    $('#switchbtn-lightmenu').prop('checked', true);
    $('#switchbtn-lightheader').prop('checked', true);
    $('#switchbtn-fullwidth').prop('checked', true);
    $('#switchbtn-fixed').prop('checked', true);
    $('#switchbtn-defaultmenu').prop('checked', true);
    $('#switchbtn-shadow').prop('checked', true);
    $('#switchbtn-defaultlogo').prop('checked', true);
    // html.setAttribute('dir', 'ltr');
    // html.setAttribute('data-theme-color', 'light');
    // html.setAttribute('data-header-style', 'light');
    // html.setAttribute('data-menu-style', 'light');
    // html.setAttribute('data-width', 'fullwidth');
    // html.setAttribute('data-position', 'fixed');
    // html.setAttribute('data-logo', 'defaultlogo');
    // html.setAttribute('data-skins', 'shadow');
    html.removeAttribute('dir', 'ltr');
    html.removeAttribute('dir', 'rtl');
    html.removeAttribute('data-theme-color', 'light');
    html.removeAttribute('data-header-style', 'light');
    html.removeAttribute('data-menu-style', 'light');
    html.removeAttribute('data-width', 'fullwidth');
    html.removeAttribute('data-position', 'fixed');
    html.removeAttribute('data-logo', 'defaultlogo');
    html.removeAttribute('data-skins', 'shadow');
    html.setAttribute('data-layout', 'vertical');
    html.setAttribute('data-vertical-style', 'default');
    document.body.classList.remove('sidenav-toggled');
    verticalFn();
    ltrFn();
    localStorage.clear();
    if (!document.body.classList.contains('login-img')) {
        let mainHeader = document.querySelector('.main-header');
        mainHeader.style = "";
        let appSidebar = document.querySelector('.app-sidebar');
        appSidebar.style = "";

        //
        checkOptions();
        menuClick();
    }
}

function checkOptions() {

    let html = document.querySelector('html')

    // dark
    if (html.getAttribute('data-theme-color') === "dark") {
        $('#switchbtn-dark').prop('checked', true);
        $('#switchbtn-darkmenu').prop('checked', false);
        $('#switchbtn-darkheader').prop('checked', false);
        $('#switchbtn-lightmenu').prop('checked', false);
        $('#switchbtn-lightheader').prop('checked', false);
        $('#switchbtn-gradientheader').prop('checked', false);
        $('#switchbtn-gradientmenu').prop('checked', false);
        $('#switchbtn-colorheader').prop('checked', false);
        $('#switchbtn-colormenu').prop('checked', false);

    }

    // horizontal
    if (html.getAttribute('data-hor-style') === "hor-click") {
        $('#switchbtn-horizontal').prop('checked', true);
    }

    // horizontal-hover
    if (html.getAttribute('data-hor-style') === "hor-hover") {
        $('#switchbtn-horizontalHover').prop('checked', true);
    }

    //RTL 
    if (html.getAttribute('dir') === "rtl") {
        $('#switchbtn-rtl').prop('checked', true);
    }

    // light header 
    if (html.getAttribute('data-header-style') === "light") {
        $('#switchbtn-lightheader').prop('checked', true);
    }

    // color header 
    if (html.getAttribute('data-header-style') === "color") {
        $('#switchbtn-colorheader').prop('checked', true);
    }

    // gradient header 
    if (html.getAttribute('data-header-style') === "gradient") {
        $('#switchbtn-gradientheader').prop('checked', true);
    }

    // dark header 
    if (html.getAttribute('data-header-style') === "dark") {
        $('#switchbtn-darkheader').prop('checked', true);
    }

    // light menu
    if (html.getAttribute('data-menu-style') === 'light') {
        $('#switchbtn-lightmenu').prop('checked', true);
    }

    // color menu
    if (html.getAttribute('data-menu-style') === 'color') {
        $('#switchbtn-colormenu').prop('checked', true);
    }

    // gradient menu
    if (html.getAttribute('data-menu-style') === 'gradient') {
        $('#switchbtn-gradientmenu').prop('checked', true);
    }

    // dark menu
    if (html.getAttribute('data-menu-style') === 'dark') {
        $('#switchbtn-darkmenu').prop('checked', true);
    }

    //boxed 
    if (html.getAttribute('data-width') === 'boxed') {
        $('#switchbtn-boxed').prop('checked', true);
    }

    //scrollable 
    if (html.getAttribute('data-position') === 'scrollable') {
        $('#switchbtn-scrollable').prop('checked', true);
    }

    //noshadow 
    if (html.getAttribute('data-skins') === 'no-shadow') {
        $('#switchbtn-noshadow').prop('checked', true);
    }

    //centerlogo 
    if (html.getAttribute('data-logo') === 'centerlogo') {
        $('#switchbtn-centerlogo').prop('checked', true);
    }

    //vertical menus

    let verticalStyles = html.getAttribute('data-vertical-style');
    switch (verticalStyles) {
        case 'default':
            $('#switchbtn-defaultmenu').prop('checked', true);
            break;
        case 'closed':
            $('#switchbtn-closed').prop('checked', true);
            break;
        case 'icontext':
            $('#switchbtn-text').prop('checked', true);
            break;
        case 'overlay':
            $('#switchbtn-overlay').prop('checked', true);
            break;
        case 'hover':
            $('#switchbtn-hoversub').prop('checked', true);
            break;
        case 'hover1':
            $('#switchbtn-hoversub1').prop('checked', true);
            break;
        case 'doublemenu':
            $('#switchbtn-doublemenu').prop('checked', true);
            break;
        case 'doublemenu-tabs':
            $('#switchbtn-doublemenu-tabs').prop('checked', true);
            break;
        default:
            $('#switchbtn-defaultmenu').prop('checked', true);
            break;

    }
}

const handleThemeUpdate = (cssVars) => {
    const root = document.querySelector(':root');
    const keys = Object.keys(cssVars);
    keys.forEach(key => {
        root.style.setProperty(key, cssVars[key]);
    });
}
const hex2rgb = (hex) => {
    const r = parseInt(hex.slice(1, 3), 16)
    const g = parseInt(hex.slice(3, 5), 16)
    const b = parseInt(hex.slice(5, 7), 16)
    return [r, g, b]
}
function dynamicPrimaryColor(primaryColor) {
    'use strict'

    primaryColor.forEach((item) => {
        item.addEventListener('input', (e) => {
            document.querySelector('html').style.setProperty('--primary-rgb', hex2rgb(e.target.value));
        });
    });
}
function dynamicBackgroundColor(imageprimaryColor) {
    'use strict'
    let html = document.querySelector('html');
    
    imageprimaryColor.forEach((item) => {
        item.addEventListener('input', (e) => {
            html.removeAttribute("data-menu-style");
            html.removeAttribute("data-header-style");
            document.querySelector('html').style.setProperty('--primary-bg-rgb', hex2rgb(e.target.value));
            document.querySelector('html').style.setProperty('--white', hex2rgb(e.target.value));
            document.querySelector('html').style.setProperty('--menu-bg', hex2rgb(e.target.value));
            document.querySelector('html').style.setProperty('--header-bg', hex2rgb(e.target.value));
            document.querySelector('html').style.setProperty('--white', hex2rgb(e.target.value));
        });
    });
}

function transparentBgColor() {
    'use strict'

    $('#switchbtn-dark').prop('checked', false);
    $('#switchbtn-colorheader').prop('checked', false);
    $('#switchbtn-colormenu').prop('checked', false);

    var userColor = document.getElementById('transparentBgColorID').value;
    localStorage.setItem('ViboontransparentBgColor', hex2rgb(userColor));

    document.querySelector('html').setAttribute("data-theme-color" , "dark");

    document.querySelector('html').removeAttribute("data-header-style" , "dark");
    document.querySelector('html').removeAttribute("data-menu-style" , "dark");

    $('#switchbtn-dark').prop('checked', true);
    checkOptions();

    localStorage.setItem('Viboondarktheme', true);
    localStorage.removeItem('ViboonlightMode');
}



function changePrimaryColor() {
    'use strict'

    checkOptions();

    var userColor = document.getElementById('colorID').value;
    localStorage.setItem('ViboonprimaryColor', hex2rgb(userColor));

    names()
}
// chart colors
let myVarVal
function names() {

    let primaryColorVal = getComputedStyle(document.documentElement).getPropertyValue('--primary-rgb').trim();

    myVarVal = localStorage.getItem("ViboonprimaryColor") || primaryColorVal;

    if (document.querySelector('#sales') !== null) {
        index(myVarVal);
    }
    
    if (document.querySelector('#projectReport') !== null) {
        index1(myVarVal);
    }
    
    if (document.querySelector('#cryptoReport') !== null) {
        index2(myVarVal);
    }
    if (document.querySelector('#ecommerce-chart-1') !== null) {
        index3(myVarVal);
    }
    if (document.querySelector('#echart') !== null){
        index4(myVarVal);
    }
    if(document.querySelector('#totalRevenueChart') !== null){
        index5(myVarVal);
    }
    if(document.querySelector('#sessionsDevice') !== null){
        index6(myVarVal);
    }
    if(document.querySelector('#statistics') !== null){
        index11(myVarVal);
    }
   
}
names()


function localStorageBackup() {
    // if there is a value stored, update color picker and background color
    // Used to retrive the data from local storage
    
    
    if (localStorage.Viboondarktheme) {
        let html = document.querySelector('html');
        html.setAttribute('data-theme-color', 'dark');
    }
    
    if (localStorage.Viboonlighttheme) {
        let html = document.querySelector('html');
        html.setAttribute('data-theme-color', 'light');
    }
    if (localStorage.Viboonrtl) {
        let html = document.querySelector('html');
        html.setAttribute('dir', 'rtl');
    }
    if (localStorage.Viboonlayout) {
        let html = document.querySelector('html');
        let layoutValue = localStorage.getItem('Viboonlayout');
        html.setAttribute('data-layout', 'horizontal');
        switch (layoutValue) {
            case 'horizontal':
                html.setAttribute('data-hor-style', 'hor-click');
                break;
            case 'horizontalhover':
                html.setAttribute('data-hor-style', 'hor-hover');
                break;
        }
    }
    if (localStorage.Viboonverticalstyles) {
        let html = document.querySelector('html');
        let verticalStyles = localStorage.getItem('Viboonverticalstyles');
        if (!(document.body.classList.contains('login-img'))) {
            switch (verticalStyles) {
                case 'closed':
                    hoverLayoutFn();
                    html.setAttribute('data-vertical-style', 'closed');
                    break;
                case 'icontext':
                    textLayoutFn();
                    html.setAttribute('data-vertical-style', 'icontext');
                    break;
                case 'overlay':
                    hoverLayoutFn();
                    html.setAttribute('data-vertical-style', 'overlay');
                    break;
                case 'hover':
                    hoverLayoutFn();
                    html.setAttribute('data-vertical-style', 'hover');
                    break;
                case 'hover1':
                    html.setAttribute('data-vertical-style', 'hover1');
                    hoverLayoutFn();
                    break;
                case 'doublemenu':
                    html.setAttribute('data-vertical-style', 'doublemenu');
                    doubleLayoutFn();
                    break;
                case 'doublemenu-tabs':
                    html.setAttribute('data-vertical-style', 'doublemenu-tabs');
                    doubleLayoutFn();
                    break;

            }
        }
    }
    if (localStorage.Viboonnoshadow) {
        let html = document.querySelector('html');
        html.setAttribute('data-skins', 'no-shadow');
    }
    if (localStorage.Viboonboxed) {
        let html = document.querySelector('html');
        html.setAttribute('data-width', 'boxed');
    }
    if (localStorage.Viboonscrollable) {
        let html = document.querySelector('html');
        html.setAttribute('data-position', 'scrollable');
    }
    if (localStorage.Vibooncenterlogo) {
        let html = document.querySelector('html');
        html.setAttribute('data-logo', 'centerlogo');
    }
    if (localStorage.ViboonMenu) {
        let html = document.querySelector('html');
        let menuValue = localStorage.getItem('ViboonMenu');
        switch (menuValue) {
            case 'light':
                html.setAttribute('data-menu-style', 'light');
                break;
            case 'dark':
                html.setAttribute('data-menu-style', 'dark');
                break;
            case 'color':
                html.setAttribute('data-menu-style', 'color');
                break;
            case 'gradient':
                html.setAttribute('data-menu-style', 'gradient');
                break;

            default:
                break;
        }
    }
    if (localStorage.ViboonHeader) {
        let html = document.querySelector('html');
        let headerValue = localStorage.getItem('ViboonHeader');
        switch (headerValue) {
            case 'light':
                html.setAttribute('data-header-style', 'light');
                break;
            case 'dark':
                html.setAttribute('data-header-style', 'dark');
                break;
            case 'color':
                html.setAttribute('data-header-style', 'color');
                break;
            case 'gradient':
                html.setAttribute('data-header-style', 'gradient');
                break;

            default:
                break;
        }
    }
    if (localStorage.ViboonprimaryColor) {
        if (document.getElementById('colorID')) {
            document.getElementById('colorID').value = localStorage.ViboonprimaryColor;
        }
        document.querySelector('html').style.setProperty('--primary-rgb', localStorage.ViboonprimaryColor);
    }
    if (localStorage.ViboontransparentBgColor) {
        document.querySelector('html').style.setProperty('--primary-bg-rgb', localStorage.ViboontransparentBgColor);
        document.querySelector('html').style.setProperty('--white', localStorage.ViboontransparentBgColor);
        document.querySelector('html').style.setProperty('--menu-bg', localStorage.ViboontransparentBgColor);
        document.querySelector('html').style.setProperty('--header-bg', localStorage.ViboontransparentBgColor);
        document.querySelector('html').style.setProperty('--white', localStorage.ViboontransparentBgColor);
        let html = document.querySelector('html');
        html.setAttribute('data-theme-color', 'dark');
        html.removeAttribute("data-menu-style");
        html.removeAttribute("data-header-style");
        $('#switchbtn-dark').prop('checked', false);
        $('#switchbtn-darkmenu').prop('checked', false);
        $('#switchbtn-darkheader').prop('checked', false);
        $('#switchbtn-lightmenu').prop('checked', false);
        $('#switchbtn-lightheader').prop('checked', false);
        $('#switchbtn-gradientheader').prop('checked', false);
        $('#switchbtn-gradientmenu').prop('checked', false);
        $('#switchbtn-colorheader').prop('checked', false);
        $('#switchbtn-colormenu').prop('checked', false);

        localStorage.removeItem("ViboonHeader", 'dark');
        localStorage.removeItem("ViboonMenu", 'dark');
        localStorage.removeItem("ViboonHeader", 'light');
        localStorage.removeItem("ViboonMenu", 'light');
    }
}


