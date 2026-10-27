//Sales Chart
function index(myVarVal) {
	var options = {
		series: [{
			name: "Orders",
			type: "column",
			data: [33, 21, 32, 37, 23, 32, 47, 31, 54, 32, 20, 38]
		}, {
			name: "Sales",
			type: "area",
			data: [44, 55, 41, 42, 22, 43, 21, 35, 56, 27, 43, 27]
		}, {
			name: "Profit",
			type: "line",
			data: [30, 25, 36, 30, 45, 35, 64, 51, 59, 36, 39, 51]
		}],
		chart: {
			height: 325,
			type: "line",
			stacked: false,
			toolbar: {
				show: false
			}
		},
		stroke: {
			width: [0, 0, 1.5],
			dashArray: [0, 0, 3],
			show: true,
			curve: 'smooth',
			lineCap: 'butt',
		},
		fill: {
			opacity: [1, .08, 1],
		},
		grid: {
			borderColor: '#f2f6f7',
		},
		xaxis: {
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
		},
		plotOptions: {
			bar: {
				columnWidth: "30%",
				borderRadius: 3
			}
		},
		options: {
			responsive: true,
			maintainAspectRatio: false,
		},
		legend: {
			position: "top"
		},
		labels: ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"],
		markers: {
			size: 0
		},
		colors: [`rgb(${myVarVal})` || "#4fb7e3", "#45aaf2", "#f6866a"]
	};
	document.getElementById('sales').innerHTML = '';
	var chart = new ApexCharts(document.querySelector("#sales"), options);
	chart.render();
}