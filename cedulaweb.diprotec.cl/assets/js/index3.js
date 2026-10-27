function index3(myVarVal) {

	var options = {
		series: [{
			name: 'Orders',
			type: 'column',
			data: [1.8, 2.5, 2.5, 1.5, 2.5, 2.8, 3.8, 4.6]
		}, {
			name: 'Sales',
			type: 'column',
			data: [1.1, 2.2, 3.1, 4, 4.1, 4.9, 6.5, 8.5]
		}, {
			name: 'Profit',
			type: 'line',
			data: [20, 29, 37, 35, 44, 43, 50, 58],
		},
		],
		chart: {
			type: 'line',
			height: 305,
			stacked: false,
			toolbar: {
				show: false,
			},
		},
		
		grid: {
			borderColor: '#f2f6f7',
			padding: {
				left: 30,
				right: 30,
			  }
		},
		dataLabels: {
			enabled: false
		},
		stroke: {
			width: [1, 1, 4],
		},
		title: {
			text: undefined,
			align: 'left',
			offsetX: 110
		},
		xaxis: {
			categories: [2015, 2016, 2017, 2018, 2019, 2020, 2021, 2022],
			axisBorder: {
				color: 'rgba(119, 119, 142, 0.05)',
				offsetX: 0,
				offsetY: 0,
			},
			axisTicks: {
				color: 'rgba(119, 119, 142, 0.05)',
				width: 6,
				offsetX: 0,
				offsetY: 0
			},
			labels: {
				rotate: -90
			}
		},
		yaxis: [
			{
				show: true,
				axisTicks: {
					show: true,
				},
				axisBorder: {
					show: false,
					color: '#4eb6d0'
				},
				labels: {
					style: {
						colors: '#4eb6d0',
					}
				},
				title: {
					text: undefined,
					style: {
						color: '#4eb6d0',
					}
				},
				tooltip: {
					enabled: true
				}
			},
			{
				seriesName: 'Orders',
				opposite: true,
				axisTicks: {
					show: true,
				},
				axisBorder: {
					show: false,
					color: '#00E396'
				},
				labels: {
					style: {
						colors: '#00E396',
					}
				},
				title: {
					text: undefined,
					style: {
						color: '#00E396',
					}
				},
			},
			{
				seriesName: 'Profit',
				opposite: true,
				axisTicks: {
					show: true,
				},
				axisBorder: {
					show: false,
					color: '#000000'
				},
				labels: {
					show: false,
					style: {
						colors: '#FEB019',
					},
				},
				title: {
					text: undefined,
					style: {
						color: '#FEB019',
					}
				}
			},
		],
		tooltip: {
			enabled: true,
		},
		colors: [`rgb(${myVarVal})` || "#4eb6d0", "#ededed", "#f6866a"],
		legend: {
			position: 'top',
			offsetX: 40
		}, stroke: {
			width: [0, 0, 1.5],
			curve: 'smooth',
			dashArray: [0, 0, 2],
		},
		plotOptions: {
			bar: {
				columnWidth: "35%",
				borderRadius: 3
			}
		},
	};
	document.getElementById('ecommerce-chart-1').innerHTML = '';
	var chart = new ApexCharts(document.querySelector("#ecommerce-chart-1"), options);
	chart.render();
}

function index4(myVarVal) {
	
	var options = {
		plotOptions: {
		  pie: {
			size: 10,
			donut: {
			  size: '70%'
			}
		  }
		},
		dataLabels: {
		  enabled: false,
		},
		series: [55, 34, 51],
		labels: ['Mobile', 'Tablet', 'Desktop'],
		chart: {
		  type: 'donut',
		  height: 220
		},
		legend: {
		  show: false
		},
		colors: [`rgb(${myVarVal})` || "#4eb6d0", '#28a745', '#eb6f33'],
		responsive: [{
		  breakpoint: 0,
		  options: {
			chart: {
			  width: 100,
			},
			legend: {
			  show: false,
			  position: 'bottom'
			}
		  },
		}]
	  };
	  document.getElementById('echart').innerHTML = '';
	  var chart = new ApexCharts(document.querySelector("#echart"), options);
	  chart.render();

}