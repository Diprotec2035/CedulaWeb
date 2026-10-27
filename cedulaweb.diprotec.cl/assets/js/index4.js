function index5(myVarVal) {

    var options = {
        series: [
            {
                name: 'This Month',
                type: 'column',
                data: [44, 55, 41, 67, 22, 43, 21, 41, 56, 27, 43, 27]
            },
            {
                name: 'Last Month',
                type: 'area',
                data: [23, 11, 22, 27, 13, 22, 37, 21, 44, 22, 45, 35]
            }
        ],
        chart: {
            height: 350,
            type: 'bar',
            fontFamily: 'Roboto, Arial, sans-serif',
            foreColor: '#7987a1',
            redrawOnWindowResize: true,
            toolbar: {
                show: false
            }
        },
        grid: {
            borderColor: '#f2f4f5',
            strokeDashArray: 3
        },
        legend: {
            position: 'top',
            fontWeight: 500,
            fontSize: '13px',
            markers: {
                radius: 20,
            },
        },
        dataLabels: {
            enabled: false
        },
        stroke: {
            curve: 'smooth',
            width: [0, 1.2],
        },
        plotOptions: {
            bar: {
                columnWidth: "20%",
                borderRadius: 1,
            }
        },
        xaxis: {
            type: 'category',
            crosshairs: {
                show: false,
            },
            axisBorder: {
                show: true,
                color: 'rgba(119, 119, 142, 0.05)',
                offsetX: 0,
                offsetY: 0,
            },
            axisTicks: {
                show: true,
                borderType: 'solid',
                color: 'rgba(119, 119, 142, 0.05)',
                width: 6,
                offsetX: 0,
                offsetY: 0
            },
            labels: {
                style: {
                    fontSize: '13px',
                    colors: '#a8b1bd',
                },
            },
            tooltip: {
                enabled: false,
            },
            labels: {
                rotate: -90
            }
        },
        labels: ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"],
        yaxis: {
            labels: {
                style: {
                    fontSize: '13px',
                    colors: '#a8b1bd',
                },
            },
        },
        tooltip: {
            shared: true,
            intersect: false,
            y: {
            formatter: function (y) {
                if(typeof y !== "undefined") {
                return  y.toFixed(0);
                }
                return y;
            }
            }
        },
        colors: [`rgb(${myVarVal})`, '#f6866a'],
        fill: {
            type: ['solid', 'gradient'],
            gradient: {
                shade: 'light',
                type: "vertical",
                opacityFrom: 0.6,
                opacityTo: 0.1,
                stops: [0, 100]
            }
        },
    };
    
    document.querySelector("#totalRevenueChart").innerHTML = '';
    var chart5 = new ApexCharts(document.querySelector("#totalRevenueChart"), options);
    chart5.render();
    }
    
    function index6(myVarVal) {
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
            height: 235
          },
          legend: {
            show: false
          },
          stroke: {
            show: true,
            colors: "#f0f0f0",
            width: 1,    
        },
          colors: [`rgb(${myVarVal})` || "#4eb6d0", '#28a745', '#f6866a'],
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
        document.getElementById('sessionsDevice').innerHTML = '';
        var chart = new ApexCharts(document.querySelector("#sessionsDevice"), options);
        chart.render();
      }
    
        var options = {
            series: [{
                name: "clients",
                data: [50, 58, 42, 48, 54, 37, 48, 41, 62, 55, 48, 52]
            }],
            chart: {
                height: 50,
                type: 'line',
                zoom: {
                    enabled: false
                },
                sparkline: {
                    enabled: true,
                },
                dropShadow: {
                    enabled: true,
                    enabledOnSeries: undefined,
                    top: 5,
                    left: 0,
                    blur: 3,
                    color:'#17b794',
                    opacity: 0.2
                }
            },
            grid: {
                show: false,
            },
            dataLabels: {
                enabled: false
            },
            title: {
                text: undefined,
            },
            xaxis: {
                crosshairs: {
                    show: false,
                }
            },
            tooltip: {
                x: {
                    show: false
                },
                y: {
                    title: {
                            formatter: function (seriesName) {
                            return ''
                        }
                    }
                },
                marker: {
                    show: false
                }
            },
            colors: ['#17b794'],
            stroke: {
                width: [2.5],
                curve: 'smooth'
            },
        };
        document.querySelector("#clientsChart").innerHTML = '';
        var chart1 = new ApexCharts(document.querySelector("#clientsChart"), options);
        chart1.render();
        
        var options = {
            series: [{
                name: "revenue",
                data: [52, 48, 60, 50, 41, 52, 37, 57, 48, 34, 58, 50]
            }],
            chart: {
                height: 50,
                type: 'line',
                zoom: {
                    enabled: false
                },
                sparkline: {
                    enabled: true,
                },
                dropShadow: {
                    enabled: true,
                    enabledOnSeries: undefined,
                    top: 5,
                    left: 0,
                    blur: 3,
                    color: '#fa713b',
                    opacity: 0.2
                }
            },
            grid: {
                show: false,
            },
            dataLabels: {
                enabled: false
            },
            title: {
                text: undefined,
            },
            xaxis: {
                crosshairs: {
                    show: false,
                }
            },
            tooltip: {
                x: {
                    show: false
                },
                y: {
                    title: {
                            formatter: function (seriesName) {
                            return ''
                        }
                    }
                },
                marker: {
                    show: false
                }
            },
            colors: ['#fa713b'],
            stroke: {
                width: [2.5],
                curve: 'smooth'
            },
        };
        var chart2 = new ApexCharts(document.querySelector("#revenueChart"), options);
        chart2.render();
        var options = {
            series: [{
                name: "sales",
                data: [50, 58, 42, 48, 64, 37, 48, 41, 62, 55, 38, 52]
            }],
            chart: {
                height: 50,
                type: 'line',
                zoom: {
                    enabled: false
                },
                sparkline: {
                    enabled: true,
                },
                dropShadow: {
                    enabled: true,
                    enabledOnSeries: undefined,
                    top: 5,
                    left: 0,
                    blur: 3,
                    color: '#05a2bf',
                    opacity: 0.2
                }
            },
            grid: {
                show: false,
            },
            dataLabels: {
                enabled: false
            },
            title: {
                text: undefined,
            },
            xaxis: {
                crosshairs: {
                    show: false,
                }
            },
            tooltip: {
                x: {
                    show: false
                },
                y: {
                    title: {
                            formatter: function (seriesName) {
                            return ''
                        }
                    }
                },
                marker: {
                    show: false
                }
            },
            colors: ['#05a2bf'],
            stroke: {
                width: [2.5],
                curve: 'smooth'
            },
        };
        var chart3 = new ApexCharts(document.querySelector("#salesChart"), options);
        chart3.render();
        var options = {
            series: [{
                name: "projects",
                data: [52, 48, 60, 50, 41, 52, 37, 57, 48, 34, 58, 50]
            }],
            chart: {
                height: 50,
                type: 'line',
                zoom: {
                    enabled: false
                },
                sparkline: {
                    enabled: true,
                },
                dropShadow: {
                    enabled: true,
                    enabledOnSeries: undefined,
                    top: 5,
                    left: 0,
                    blur: 3,
                    color: '#fca13a',
                    opacity: 0.2
                }
            },
            grid: {
                show: false,
            },
            dataLabels: {
                enabled: false
            },
            title: {
                text: undefined,
            },
            xaxis: {
                crosshairs: {
                    show: false,
                }
            },
            tooltip: {
                x: {
                    show: false
                },
                y: {
                    title: {
                            formatter: function (seriesName) {
                            return ''
                        }
                    }
                },
                marker: {
                    show: false
                }
            },
            colors: ['#fca13a'],
            stroke: {
                width: [2.5],
                curve: 'smooth'
            },
        };
        var chart4 = new ApexCharts(document.querySelector("#projectsChart"), options);
        chart4.render();