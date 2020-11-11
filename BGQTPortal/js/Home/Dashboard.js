angular.module('app').controller('hdbCtrl', function (AuthVerify, $scope, $http, $state, $stateParams, $location, $window) {
    $scope.UserCounts = '';

    if ($window.innerWidth >= 1024) {
        $(".dashboarddiv").css("height", $window.innerHeight - 84);
    } else {
        $(".dashboarddiv").css("height", $window.innerHeight - 4);
    }
    window.onresize = function () {
        $scope.$apply(function () {
            if ($window.innerWidth >= 1024) {
                $(".dashboarddiv").css("height", $window.innerHeight - 84);
            } else {
                $(".dashboarddiv").css("height", $window.innerHeight - 4);
            }
        })
    };

    $scope.GetSystemCount = function () {

        $http({
            method: 'GET',
            url: storage.getItem("InterfaceUrl") + 'SystemReport.ashx?op=GetSystemUserCount&accessToken=' + storage.getItem("userAdminToken"),
        }).then(function successCallback(response) {
            var data = response.data;
            if (data.result != "false") {
                $scope.UserCounts = data.data.Conut;
             } else {
                ErrorHandling(data.result, data.errCode, data.errMsg);
            }
        }, function errorCallback(e) {
        });

         //邮件总数详情
        $.getJSON(storage.getItem("InterfaceUrl") + 'SystemReport.ashx?op=GetSystemMailBoxCount&accessToken=' + storage.getItem("userAdminToken"), function (json) {
            var SendCount = [];
            var ReceiveCount = [];
            var TotaCount = [];
            var MonthCount = 0;
            var YearCount = 0;
            var tMonth = new Date().getMonth() + 1;
            for (var j = 0; j < json.data.length; j++) {

                SendCount.push(json.data[j].SendMailCount);
                ReceiveCount.push(json.data[j].ReceiveMailCount);
                TotaCount.push(json.data[j].TotalMailCount);
                YearCount += json.data[j].TotalMailCount;
                if (json.data[j].month == tMonth) {
                    MonthCount = json.data[j].TotalMailCount;
                }
            }
            $('#systemmail').highcharts({
                chart: {
                    zoomType: 'xy'
                },
                title: {
                    text: '邮件总数详情',
                    style: { fontSize: '16px' }
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
                    //backgroundColor: (Highcharts.theme && Highcharts.theme.legendBackgroundColor) || '#FFFFFF'
                },
                series: [{
                    name: '总量',
                    type: 'column',
                    yAxis: 1,
                    data: TotaCount,
                    tooltip: {
                        valueSuffix: ' 封'
                    }
                }, {
                    name: '接收邮件',
                    type: 'spline',
                    yAxis: 2,
                    data: ReceiveCount,
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
                    data: SendCount,
                    tooltip: {
                        valueSuffix: ' 封'
                    }
                }]
            });

            $('#mailRATIO').highcharts({
                chart: {
                    plotBackgroundColor: null,
                    plotBorderWidth: null,
                    plotShadow: false,
                    type: 'pie'
                },
                title: {
                    floating: true,
                    text: tMonth + '月邮件数占比',
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
                            name: tMonth + '月',
                            y: MonthCount,
                            sliced: true,
                            selected: true
                        },
                        ['其他', YearCount - MonthCount]
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
        });

        $.getJSON(storage.getItem("InterfaceUrl") + 'SystemReport.ashx?op=GetEntryAndDepartureUserCount&accessToken=' + storage.getItem("userAdminToken"), function (json) {
            var EntryCount = [];
            var DepartureCount = [];
            for (var j = 0; j < json.data.length; j++) {
                EntryCount.push(json.data[j].EntryCount);
                DepartureCount.push(json.data[j].DepartureCount);
            }
            $('#container').highcharts({
                chart: {
                    type: 'column'
                },
                title: {
                    text: '用户统计',
                    style: { fontSize: '16px' }
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
                    data: EntryCount
                }, {
                    name: '离职用户',
                    data: DepartureCount
                }],   //此处定义两个series，即两条线，最高气温和最低气温，如果更多则在里面添加大括号。
            });
        });

        //已用空间统计
        $.getJSON(storage.getItem("InterfaceUrl") + 'SystemReport.ashx?op=GetUserUsedMailBoxList&accessToken=' + storage.getItem("userAdminToken"), function (json) {
            var userlist = [];
            var usedmailsize = [];
            var usablemailsize = [];
            for (var j = 0; j < json.data.length; j++) {
                //userlist.push(json.data[j].displayname + "(" + json.data[j].mail + ")");
                userlist.push(json.data[j].displayname);
                usedmailsize.push(json.data[j].usedMailSize);
                usablemailsize.push(json.data[j].usableMailSize);
            }
            $('#container1').highcharts({
                chart: {
                    type: 'column'
                },
                title: {
                    text: '用户已用空间排名',
                    style: { fontSize: '16px' }
                },
                credits: {
                    enabled: false
                },
                xAxis: {
                    categories: userlist
                },
                yAxis: {
                    min: 0,
                    title: {
                        text: '邮箱空间量'
                    },
                    //stackLabels: {  // 堆叠数据标签
                    //    enabled: true,
                    //    style: {
                    //        fontWeight: 'bold',
                    //        color: (Highcharts.theme && Highcharts.theme.textColor) || 'gray'
                    //    }
                    //}
                },
                tooltip: {
                    formatter: function () {
                        return '<b>' + this.x + '</b><br/>' +
                            this.series.name + ': ' + this.y + '<br/>';
                    }
                },
                plotOptions: {
                    series: {
                        stacking: 'normal'
                    }
                },
                series: [{
                    name: '可用空间(MB)',
                    data: usablemailsize
                }, {
                    name: '已用空间(MB)',
                    data: usedmailsize
                }]

            });
        });

        //MailboxDB统计
        $.getJSON(storage.getItem("InterfaceUrl") + 'SystemReport.ashx?op=GetMailBoxDBUsedList&accessToken=' + storage.getItem("userAdminToken"), function (json) {
            var mailboxdblist = [];
            var usercount = [];
            var usedmailsize = [];
            for (var j = 0; j < json.data.length; j++) {
                mailboxdblist.push(json.data[j].mailboxdbname);
                usercount.push(json.data[j].usercount);
                usedmailsize.push(json.data[j].usedmailsize);
            }
            $('#container2').highcharts({
                chart: {
                    type: 'bar' //bar
                },
                title: {
                    text: 'MailBoxDB使用量',
                    style: { fontSize: '16px' }
                },
                credits: {
                    enabled: false
                },
                xAxis: {
                    categories: mailboxdblist
                },
                yAxis: {
                    min: 0,
                    title: {
                        text: '用户数量',
                        align: 'high'
                    },
                    labels: {
                        overflow: 'justify'
                    }
                },
                tooltip: {
                    valueSuffix: ' 人'
                },
                plotOptions: {
                    bar: {
                        dataLabels: {
                            enabled: true,
                            allowOverlap: true // 允许数据标签重叠
                        }
                    }
                },
                legend: {
                    //layout: 'vertical',
                    //align: 'right',
                    //verticalAlign: 'top',
                    //x: 10,
                    //y: 100,
                    //floating: true,
                    //borderWidth: 1,
                    //backgroundColor: ((Highcharts.theme && Highcharts.theme.legendBackgroundColor) || '#FFFFFF'),
                    shadow: false
                },
                series: [{
                    name: '用户数量',
                    data: usercount
                }]
            });
        });

        $.getJSON(storage.getItem("InterfaceUrl") + 'SystemReport.ashx?op=GetCompanyMailCount&accessToken=' + storage.getItem("userAdminToken"), function (json) {
            var data1 = [];
            for (var j = 0; j < json.data.length; j++) {
                data1.push([json.data[j].HPS_WORK_COMP_DESC, json.data[j].MailCount]);
            }
            $('#company').highcharts({
                chart: {
                    plotBackgroundColor: null,
                    plotBorderWidth: null,
                    plotShadow: false,
                    type: 'pie',
                    marginRight: 120
                },
                legend: {
                    layout: 'vertical',
                    align: 'right',
                    verticalAlign: 'middle',
                    x: -60,
                    y: 20
                },
                title: {
                    text: '公司邮件数量排名（前10名）',
                    style: { fontSize: '16px' }
                },
                credits: {
                    enabled: false
                },
                tooltip: {
                    pointFormat: '{series.name}: <b>{point.y}</b>'
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
                loading: {
                    style: {
                        position: 'absolute',//默认值
                        opacity: 0.5,//透明度
                        textAlign: 'center',//文字显示方式
                        backgroundColor: 'gray',//背景色
                        //backgroundImage: 'url("../Content/images/drip.gif")',//显示的背景动态gif图片，设置此项为gif图片即可实现常见的加载中动态效果图
                        backgroundSize: '100% 100%'//设置背景图片铺满Series数据列区域
                    }
                },
                series: [{
                    name: '邮件数量',
                    colorByPoint: true,
                    data: data1
                }]
            });
        });

        $.getJSON(storage.getItem("InterfaceUrl") + 'SystemReport.ashx?op=GetDeptMailCount&accessToken=' + storage.getItem("userAdminToken"), function (json) {
            var data1 = [];
            for (var j = 0; j < json.data.length; j++) {
                var name = json.data[j].DEPT_DESCR + '(' + json.data[j].COMPANY_DESCR + ')';
                data1.push([name, json.data[j].MailCount]);
            }
            $('#dept').highcharts({
                chart: {
                    plotBackgroundColor: null,
                    plotBorderWidth: null,
                    plotShadow: false,
                    type: 'pie',
                    marginRight: 120
                },
                legend: {
                    layout: 'vertical',
                    align: 'right',
                    verticalAlign: 'middle',
                    x: 60,
                    y: 20
                },
                title: {
                    text: '部门邮件数量排名（前10名）',
                    style: { fontSize: '16px' }
                },
                credits: {
                    enabled: false
                },
                tooltip: {
                    pointFormat: '{series.name}: <b>{point.y}</b>'
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
                loading: {
                    hideDuration: 100,
                    labelStyle: { "fontWeight": "bold", "position": "relative", "top": "45%" },
                    showDuration: 100,
                    style: { "position": "absolute", "backgroundColor": "#ffffff", "opacity": 0.5, "textAlign": "center" }
                },
                series: [{
                    name: '邮件数量',
                    colorByPoint: true,
                    data: data1
                }]
            });
        });

        $.getJSON(storage.getItem("InterfaceUrl") + 'SystemReport.ashx?op=GetUserMailCount&accessToken=' + storage.getItem("userAdminToken"), function (json) {
            var data1 = [];
            for (var j = 0; j < json.data.length; j++) {
                var name = json.data[j].NAME + '(' + json.data[j].EMPLID + ')';
                data1.push([name, json.data[j].MailCount]);
            }
            $('#user').highcharts({
                chart: {
                    plotBackgroundColor: null,
                    plotBorderWidth: null,
                    plotShadow: false,
                    type: 'pie',
                    marginRight: 120
                },
                legend: {
                    layout: 'vertical',
                    align: 'right',
                    verticalAlign: 'middle',
                    x: 60,
                    y: 20
                },
                title: {
                    text: '用户邮件数量排名（前10名）',
                    style: { fontSize: '16px' }
                },
                credits: {
                    enabled: false
                },
                tooltip: {
                    pointFormat: '{series.name}: <b>{point.y}</b>'
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
                    name: '邮件数量',
                    colorByPoint: true,
                    data: data1
                }]
            });
        });
    };

   
})

