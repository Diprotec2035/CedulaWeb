(function ($) {
	"use strict";

	//P-scrolling
	const ps5 = new PerfectScrollbar('.notification-list', {
		useBothWheelAxes: true,
		suppressScrollX: true,
	});

	//P-scrolling
	const ps6 = new PerfectScrollbar('.cart-list', {
		useBothWheelAxes: true,
		suppressScrollX: true,
	});

	//P-scrolling
	// const ps16 = new PerfectScrollbar('.slide-menu', {
	// 	useBothWheelAxes: true,
	// 	suppressScrollX: true,
	// });
	
	$('.tabs-menu-body').each(function () { const ps = new PerfectScrollbar($(this)[0]); });

})(jQuery);