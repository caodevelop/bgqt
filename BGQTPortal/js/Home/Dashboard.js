angular.module('app').controller('hdbCtrl', function (AuthVerify, $scope, $http, $state, $stateParams, $location, $window) {
    // 邮件总量
    $('#systemmail').highcharts({
        chart: {
            zoomType: 'xy'
        },
        title: {
            text: '邮件总数详情'
        },
        credits: {
            enabled: false
        },
        xAxis: [{
            categories: [
                '一月', '二月', '三月', '四月', '五月', '六月', '七月', '八月', '九月', '十月', '十一月', '十二月'
            ],
            crosshair: true
        }],
        yAxis: [{ // Primary yAxis
            labels: {
                format: '{value} 封',
                style: {
                    color: Highcharts.getOptions().colors[2]
                }
            },
            title: {
                text: '发送邮件数量',
                style: {
                    color: Highcharts.getOptions().colors[2]
                }
            },
            opposite: true
        }, { // Secondary yAxis
            gridLineWidth: 0,
            title: {
                text: '总量',
                style: {
                    color: Highcharts.getOptions().colors[0]
                }
            },
            labels: {
                format: '{value} 封',
                style: {
                    color: Highcharts.getOptions().colors[0]
                }
            }
        }, { // Tertiary yAxis
            gridLineWidth: 0,
            title: {
                text: '接收邮件数量',
                style: {
                    color: Highcharts.getOptions().colors[1]
                }
            },
            labels: {
                format: '{value} 封',
                style: {
                    color: Highcharts.getOptions().colors[1]
                }
            },
            opposite: true
        }],
        tooltip: {
            shared: true
        },
        legend: {
            layout: 'vertical',
            align: 'left',
            x: 80,
            verticalAlign: 'top',
            y: 55,
            floating: true,
            backgroundColor: (Highcharts.theme && Highcharts.theme.legendBackgroundColor) || '#FFFFFF'
        },
        series: [{
            name: '总量',
            type: 'column',
            yAxis: 1,
            data: [1050, 800, 600, 1200, 1234, 100, 200, 500, 300, 200, 500, 480],
            tooltip: {
                valueSuffix: ' 封'
            }
        }, {
            name: '接收邮件',
            type: 'spline',
            yAxis: 2,
            data: [800, 600, 300, 500, 400, 90, 130, 300, 100, 150, 369, 10],
            marker: {
                enabled: false
            },
            dashStyle: 'shortdot',
            tooltip: {
                valueSuffix: ' 封'
            }
        }, {
            name: '发送邮件',
            type: 'spline',
                data: [200, 100, 200, 440, 80, 10, 300, 200, 90, 50, 39, 400],
            tooltip: {
                valueSuffix: ' 封'
            }
        }]
    });
    //邮件占比
    $('#mailRATIO').highcharts({
        chart: {
            plotBackgroundColor: null,
            plotBorderWidth: null,
            plotShadow: false,
            type: 'pie'
        },
        title: {
            floating: true,
            text: '7月邮件数占比',
            style: {
                color: '#3E576F',
                fontSize: '12px'
            }
        },
        credits: {
            enabled: false
        },
      

        plotOptions: {
            pie: {
                allowPointSelect: true,
                cursor: 'pointer',
                dataLabels: {
                    enabled: false
                },
                showInLegend: true
            }
        },
        series: [{
            type: 'pie',
            innerSize: '80%',
            name: '邮件占比',
            data: [
                {
                    name: '7月',
                    y: 23,
                    sliced: true,
                    selected: true
                },
                ['其他', 77]
            ]
        }]
    }, function (c) { // 图表初始化完毕后的会掉函数
        // 环形图圆心
        var centerY = c.series[0].center[1],
            titleHeight = parseInt(c.title.styles.fontSize);
        // 动态设置标题位置
        c.setTitle({
            y: centerY + titleHeight / 2
        });
    });


    $('#container').highcharts({
        chart: {
            type: 'column'
        },
        title: {
            text: '用户统计'
        },
        credits: {
            enabled: false
        },
        xAxis: {
            categories: [
                '一月', '二月', '三月', '四月', '五月', '六月', '七月', '八月', '九月', '十月', '十一月', '十二月'
            ],
            crosshair: true
        },
        yAxis: {
            min: 0,
            title: {
                text: '用户数量'
            }
        },
        series: [{
            name: '入职用户',
            data: [3, 0, 10, 2, 0, 0, 0, 0, 1, 5, 0, 0]
        }, {
            name: '离职用户',
                data: [4, 2, 6, 1, 0, 2, 0, 4, 0, 3, 0, 0]
        }]
     });

    $('#container1').highcharts({
        chart: {
            type: 'bar'
        },
        title: {
            text: '用户已用空间排名'
        },
        credits: {
            enabled: false
        },
        xAxis: {
            categories: ['张三', '李四', '王五', '赵六']
        },
        yAxis: {
            min: 0,
            title: {
                text: '邮箱空间量'
            }
        },
        legend: {
            /* 图例显示顺序反转
             * 这是因为堆叠的顺序默认是反转的，可以设置 
             * yAxis.reversedStacks = false 来达到类似的效果 
             */
            reversed: true
        },
        plotOptions: {
            series: {
                stacking: 'normal'
            }
        },
        series: [{
            name: '可用空间',
            data: [8, 8, 7, 8, 9]
        }, {
            name: '已用空间',
            data: [2, 2, 3, 2, 1]
        }]

    });
    $('#container2').highcharts({
        chart: {
            type: 'column'
        },
        title: {
            text: 'MailBoxDB空间量'
        },
        xAxis: {
            categories: ['MailBoxDB01', 'MailBoxDB02', 'MailBoxDB03', 'MailBoxDB04',
                'MailBoxDB05', 'MailBoxDB06', 'MailBoxDB07', 'MailBoxDB08']
        },
        yAxis: {
            min: 0,
            title: {
                text: '邮箱空间量'
            },
            stackLabels: {  // 堆叠数据标签
                enabled: true,
                style: {
                    fontWeight: 'bold',
                    color: (Highcharts.theme && Highcharts.theme.textColor) || 'gray'
                }
            }
        },
        legend: {
            align: 'right',
            x: -30,
            verticalAlign: 'top',
            y: 25,
            floating: true,
            backgroundColor: (Highcharts.theme && Highcharts.theme.background2) || 'white',
            borderColor: '#CCC',
            borderWidth: 1,
            shadow: false
        },
        tooltip: {
            formatter: function () {
                return '<b>' + this.x + '</b><br/>' +
                    this.series.name + ': ' + this.y + '<br/>' +
                    '总量: ' + this.point.stackTotal;
            }
        },
        plotOptions: {
            column: {
                stacking: 'normal',
                dataLabels: {
                    enabled: true,
                    color: (Highcharts.theme && Highcharts.theme.dataLabelsColor) || 'white',
                    style: {
                        // 如果不需要数据标签阴影，可以将 textOutline 设置为 'none'
                        textOutline: '1px 1px black'
                    }
                }
            }
        },
        series: [{
            name: '可用空间',
            data: [8, 8, 7, 8, 9,5,7,9]
        }, {
            name: '已用空间',
            data: [2, 2, 3, 2, 1,5,3,1]
        }]
    });

    $('#company').highcharts({
        chart: {
            plotBackgroundColor: null,
            plotBorderWidth: null,
            plotShadow: false,
            type: 'pie'
        },
        title: {
            text: '公司'
        },
        credits: {
            enabled: false
        },
        tooltip: {
            pointFormat: '{series.name}: <b>{point.percentage:.1f}%</b>'
        },
        plotOptions: {
            pie: {
                allowPointSelect: true,
                cursor: 'pointer',
                dataLabels: {
                    enabled: false
                },
                showInLegend: true
            }
        },
        series: [{
            name: 'Brands',
            colorByPoint: true,
            data: [{
                name: 'Chrome',
                y: 61.41,
                sliced: true,
                selected: true
            }, {
                name: 'Internet Explorer',
                y: 11.84
            }, {
                name: 'Firefox',
                y: 10.85
            }, {
                name: 'Edge',
                y: 4.67
            }, {
                name: 'Safari',
                y: 4.18
            }, {
                name: 'Other',
                y: 7.05
            }]
        }]
    });

    $('#dept').highcharts({
        chart: {
            plotBackgroundColor: null,
            plotBorderWidth: null,
            plotShadow: false,
            type: 'pie'
        },
        title: {
            text: '部门'
        },
        credits: {
            enabled: false
        },
        tooltip: {
            pointFormat: '{series.name}: <b>{point.percentage:.1f}%</b>'
        },
        plotOptions: {
            pie: {
                allowPointSelect: true,
                cursor: 'pointer',
                dataLabels: {
                    enabled: false
                },
                showInLegend: true
            }
        },
        series: [{
            name: 'Brands',
            colorByPoint: true,
            data: [{
                name: 'Chrome',
                y: 61.41,
                sliced: true,
                selected: true
            }, {
                name: 'Internet Explorer',
                y: 11.84
            }, {
                name: 'Firefox',
                y: 10.85
            }, {
                name: 'Edge',
                y: 4.67
            }, {
                name: 'Safari',
                y: 4.18
            }, {
                name: 'Other',
                y: 7.05
            }]
        }]
    });

    $('#user').highcharts({
        chart: {
            plotBackgroundColor: null,
            plotBorderWidth: null,
            plotShadow: false,
            type: 'pie'
        },
        title: {
            text: '用户'
        },
        credits: {
            enabled: false
        },
        tooltip: {
            pointFormat: '{series.name}: <b>{point.percentage:.1f}%</b>'
        },
        plotOptions: {
            pie: {
                allowPointSelect: true,
                cursor: 'pointer',
                dataLabels: {
                    enabled: false
                },
                showInLegend: true
            }
        },
        series: [{
            name: 'Brands',
            colorByPoint: true,
            data: [{
                name: 'Chrome',
                y: 61.41,
                sliced: true,
                selected: true
            }, {
                name: 'Internet Explorer',
                y: 11.84
            }, {
                name: 'Firefox',
                y: 10.85
            }, {
                name: 'Edge',
                y: 4.67
            }, {
                name: 'Safari',
                y: 4.18
            }, {
                name: 'Other',
                y: 7.05
            }]
        }]
    });
})

