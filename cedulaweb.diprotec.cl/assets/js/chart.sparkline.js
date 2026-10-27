$(function() {
	'use strict'
	
	/***************** LINE CHARTS *****************/
	$('#sparkline1').sparkline('html', {
		width: 200,
		height: 70,
		lineColor: '#17b794',
		fillColor: false,
	});
	$('#sparkline2').sparkline('html', {
		width: 200,
		height: 70,
		lineColor: '#eb6f33',
		fillColor: false
	});
	$('#sparkline11').sparkline('html', {
		width: 200,
		height: 70,
		lineColor: '#bc299a',
		fillColor: false,
	});
	/************** AREA CHARTS ********************/
	$('#sparkline3').sparkline('html', {
		width: 200,
		height: 70,
		lineColor: '#17b794',
		fillColor: 'rgba(133, 0, 255, 0.2)',
	});
	$('#sparkline4').sparkline('html', {
		width: 200,
		height: 70,
		lineColor: '#17b794',
		fillColor: 'rgba(76, 190, 150, 0.2)'
	});
	$('#sparkline14').sparkline('html', {
		width: 200,
		height: 70,
		lineColor: '#bc299a',
		fillColor: 'rgba(244, 187, 231, 0.2)'
	});
	/******************* BAR CHARTS *****************/
	$('#sparkline5').sparkline('html', {
		type: 'bar',
		barWidth: 10,
		height: 70,
		barColor: '#17b794',
		chartRangeMax: 12
	});
	$('#sparkline6').sparkline('html', {
		type: 'bar',
		barWidth: 10,
		height: 70,
		barColor: '#285cf7',
		chartRangeMax: 12
	});
	$('#sparkline16').sparkline('html', {
		type: 'bar',
		barWidth: 10,
		height: 70,
		barColor: '#bc299a',
		chartRangeMax: 12
	});
	/***************** STACKED BAR CHARTS ****************/
	$('#sparkline7').sparkline('html', {
		type: 'bar',
		barWidth: 10,
		height: 70,
		barColor: '#eb6f33',
		chartRangeMax: 12
	});
	$('#sparkline7').sparkline([4, 5, 6, 7, 4, 5, 8, 7, 6, 6, 4, 7, 6, 4, 7], {
		composite: true,
		type: 'bar',
		barWidth: 10,
		height: 70,
		barColor: '#17b794',
		chartRangeMax: 12
	});
	$('#sparkline8').sparkline('html', {
		type: 'bar',
		barWidth: 10,
		height: 70,
		barColor: '#eb6f33',
		chartRangeMax: 12
	});
	$('#sparkline8').sparkline([4, 5, 6, 7, 4, 5, 8, 7, 6, 6, 4, 7, 6, 4, 7], {
		composite: true,
		type: 'bar',
		barWidth: 10,
		height: 70,
		barColor: '#17b794 ',
		chartRangeMax: 12
	});
	$('#sparkline18').sparkline('html', {
		type: 'bar',
		barWidth: 10,
		height: 70,
		barColor: '#bc299a',
		chartRangeMax: 12
	});
	$('#sparkline18').sparkline([4, 5, 6, 7, 4, 5, 8, 7, 6, 6, 4, 7, 6, 4, 7], {
		composite: true,
		type: 'bar',
		barWidth: 10,
		height: 70,
		barColor: '#eb6f33',
		chartRangeMax: 12
	});
	/**************** PIE CHART ****************/
	$('#sparkline9').sparkline('html', {
		type: 'pie',
		width: 70,
		height: 70,
		sliceColors: ['#17b794', '#eb6f33', '#28a745']
	});
	$('#sparkline10').sparkline('html', {
		type: 'pie',
		width: 70,
		height: 70,
		sliceColors: ['#17b794', '#eb6f33', '#28a745']
	});
	$('#sparkline01').sparkline('html', {
		type: 'pie',
		width: 70,
		height: 70,
		sliceColors: ['#17b794',  '#28a745','#ffc107','#3db4ec','#dc3545']
	});
	$('#sparkline02').sparkline('html', {
		type: 'pie',
		width: 70,
		height: 70,
		sliceColors: ['#bc299a', '#eb6f33', '#28a745','#17b794 ','#ffc107','#bc299a','#dc3545']
	});
});