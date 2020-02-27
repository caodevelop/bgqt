app.config(function ($stateProvider, $urlRouterProvider) {
    $urlRouterProvider.otherwise('/logOnOU');
    $stateProvider.state('logOnOU', {
        url: '/logOnOU',
        templateUrl: 'Admin/logOnAdmin.html?timestamp=' + new Date().getTime(),
        controller: 'logOnAdminCtrl',
        resolve: {
            loadMyCtrl: ['$ocLazyLoad', function ($ocLazyLoad) {
                return $ocLazyLoad.load('js/Admin/logOnAdmin.js?timestamp=' + new Date().getTime()),
                    $ocLazyLoad.load('css/common.css?timestamp=' + new Date().getTime());
            }]
        },
        cache: false,
    }).state('resetPwd', {
        url: '/resetPwd',
        templateUrl: 'Admin/resetPwd.html?timestamp=' + new Date().getTime(),
        controller: 'resetPwdCtrl',
        resolve: {
            loadMyCtrl: ['$ocLazyLoad', function ($ocLazyLoad) {
                return $ocLazyLoad.load('js/Admin/resetPwd.js?timestamp=' + new Date().getTime()),
                    $ocLazyLoad.load('css/common.css?timestamp=' + new Date().getTime());
            }]
        },
        cache: false,
    }).state('Home', {
        url: '/Home',
        //params: { "type": null },
        templateUrl: 'Home/Home.html?timestamp=' + new Date().getTime(),
        controller: 'homeCtrl',
        resolve: {
            loadMyCtrl: ['$ocLazyLoad', function ($ocLazyLoad) {
                return $ocLazyLoad.load('js/Home/Home.js?timestamp=' + new Date().getTime())
            }]
        },
        cache: false,
    })
        .state('Home.Org', {
            url: '/HO',
            templateUrl: 'OrgStructure/Org.html?timestamp=' + new Date().getTime(),
            controller: 'OrgCtrl',
            resolve: {
                loadMyCtrl: ['$ocLazyLoad', function ($ocLazyLoad) {
                    return $ocLazyLoad.load('js/OrgStructure/Org.js?timestamp=' + new Date().getTime())
                }]
            },
            cache: false,
        })
        //.state('Home.HAB', {
        //    url: '/HHAB',
        //    templateUrl: 'OrgStructure/HAB.html?timestamp=' + new Date().getTime(),
        //    controller: 'HABCtrl',
        //    resolve: {
        //        loadMyCtrl: ['$ocLazyLoad', function ($ocLazyLoad) {
        //            return $ocLazyLoad.load('js/OrgStructure/HAB.js?timestamp=' + new Date().getTime())
        //        }]
        //    },
        //    cache: false,
        //})
        //.state('Home.PAG', {
        //    url: '/HPAG',
        //    params: { "type": null },
        //    templateUrl: 'OrgStructure/PublicAccountGroup.html?timestamp=' + new Date().getTime(),
        //    controller: 'PAGCtrl',
        //    resolve: {
        //        loadMyCtrl: ['$ocLazyLoad', function ($ocLazyLoad) {
        //            return $ocLazyLoad.load('js/OrgStructure/PublicAccountGroup.js?timestamp=' + new Date().getTime())
        //        }]
        //    },
        //    cache: false,
        //})
        .state('Home.Quit', {
            url: '/HQ',
            //params: { "ViewDetailsID": null },
            templateUrl: 'OrgStructure/Quit.html?timestamp=' + new Date().getTime(),
            controller: 'QuitCtrl',
            resolve: {
                loadMyCtrl: ['$ocLazyLoad', function ($ocLazyLoad) {
                    return $ocLazyLoad.load('js/OrgStructure/Quit.js?timestamp=' + new Date().getTime())
                }]
            },
            cache: false,
        })
        //.state('Home.SCG', {
        //    url: '/HSCG',
        //    params: { "type": null },
        //    templateUrl: 'OrgStructure/StaticCommunicationsGroup.html?timestamp=' + new Date().getTime(),
        //    controller: 'SCGCtrl',
        //    resolve: {
        //        loadMyCtrl: ['$ocLazyLoad', function ($ocLazyLoad) {
        //            return $ocLazyLoad.load('js/OrgStructure/StaticCommunicationsGroup.js?timestamp=' + new Date().getTime())
        //        }]
        //    },
        //    cache: false,
        //})
        //.state('Home.SOU', {
        //    url: '/HSOU',
        //    params: { "type": null },
        //    templateUrl: 'OrgStructure/SameOU.html?timestamp=' + new Date().getTime(),
        //    controller: 'SOUCtrl',
        //    resolve: {
        //        loadMyCtrl: ['$ocLazyLoad', function ($ocLazyLoad) {
        //            return $ocLazyLoad.load('js/OrgStructure/SameOU.js?timestamp=' + new Date().getTime())
        //        }]
        //    },
        //    cache: false,
        //})
        .state('Home.CreateUser', {
            url: '/HCU',
            params: { "type": null },
            templateUrl: 'SystemStatistics/CreatedUser.html?timestamp=' + new Date().getTime(),
            controller: 'CUCtrl',
            resolve: {
                loadMyCtrl: ['$ocLazyLoad', function ($ocLazyLoad) {
                    return $ocLazyLoad.load('js/SystemStatistics/CreatedUser.js?timestamp=' + new Date().getTime())
                }]
            },
            cache: false,
        }).state('Home.Deactivated', {
            url: '/HD',
            params: { "type": null },
            templateUrl: 'SystemStatistics/Deactivated.html?timestamp=' + new Date().getTime(),
            controller: 'DeactivatedCtrl',
            resolve: {
                loadMyCtrl: ['$ocLazyLoad', function ($ocLazyLoad) {
                    return $ocLazyLoad.load('js/SystemStatistics/Deactivated.js?timestamp=' + new Date().getTime())
                }]
            },
            cache: false,
        }).state('Home.NeverLogin', {
            url: '/HNL',
            params: { "type": null },
            templateUrl: 'SystemStatistics/NeverLogin.html?timestamp=' + new Date().getTime(),
            controller: 'NLCtrl',
            resolve: {
                loadMyCtrl: ['$ocLazyLoad', function ($ocLazyLoad) {
                    return $ocLazyLoad.load('js/SystemStatistics/NeverLogin.js?timestamp=' + new Date().getTime())
                }]
            },
            cache: false,
        }).state('Home.PVP', {
            url: '/HPVP',
            params: { "type": null },
            templateUrl: 'SystemStatistics/PwdValidityPeriod.html?timestamp=' + new Date().getTime(),
            controller: 'PVPCtrl',
            resolve: {
                loadMyCtrl: ['$ocLazyLoad', function ($ocLazyLoad) {
                    return $ocLazyLoad.load('js/SystemStatistics/PwdValidityPeriod.js?timestamp=' + new Date().getTime())
                }]
            },
            cache: false,
        })
        //.state('Home.MA', {
        //    url: '/HMA',
        //    params: { "type": null },
        //    templateUrl: 'SystemManagement/MailApproval.html?timestamp=' + new Date().getTime(),
        //    controller: 'MACtrl',
        //    resolve: {
        //        loadMyCtrl: ['$ocLazyLoad', function ($ocLazyLoad) {
        //            return $ocLazyLoad.load('js/SystemManagement/MailApproval.js?timestamp=' + new Date().getTime())
        //        }]
        //    },
        //    cache: false,
        //})
        //.state('Home.MD', {
        //    url: '/HMD',
        //    params: { "type": null },
        //    templateUrl: 'SystemManagement/MailboxDatabase.html?timestamp=' + new Date().getTime(),
        //    controller: 'MDCtrl',
        //    resolve: {
        //        loadMyCtrl: ['$ocLazyLoad', function ($ocLazyLoad) {
        //            return $ocLazyLoad.load('js/SystemManagement/MailboxDatabase.js?timestamp=' + new Date().getTime())
        //        }]
        //    },
        //    cache: false,
        //})
        .state('Home.RP', {
            url: '/HRP',
            params: { "type": null },
            templateUrl: 'SystemManagement/RolesAndPermissions.html?timestamp=' + new Date().getTime(),
            controller: 'RPCtrl',
            resolve: {
                loadMyCtrl: ['$ocLazyLoad', function ($ocLazyLoad) {
                    return $ocLazyLoad.load('js/SystemManagement/RolesAndPermissions.js?timestamp=' + new Date().getTime())
                }]
            },
            cache: false,
        })
        .state('Home.SM', {
            url: '/HSM',
            params: { "type": null },
            templateUrl: 'SystemManagement/SensitiveMail.html?timestamp=' + new Date().getTime(),
            controller: 'SMCtrl',
            resolve: {
                loadMyCtrl: ['$ocLazyLoad', function ($ocLazyLoad) {
                    return $ocLazyLoad.load('js/SystemManagement/SensitiveMail.js?timestamp=' + new Date().getTime())
                }]
            },
            cache: false,
        })
        .state('Home.Log', {
            url: '/HLG',
            params: { "type": null },
            templateUrl: 'SystemLog/log.html?timestamp=' + new Date().getTime(),
            controller: 'logCtrl',
            resolve: {
                loadMyCtrl: ['$ocLazyLoad', function ($ocLazyLoad) {
                    return $ocLazyLoad.load('js/SystemLog/log.js?timestamp=' + new Date().getTime())
                }]
            },
            cache: false,
        })
});
// 禁止模板缓存 
app.run(function ($rootScope, $templateCache) {
    $rootScope.$on('$routeChangeStart', function (event, next, current) {
        if (typeof (current) !== 'undefined') {
            $templateCache.remove(current.templateUrl);
        }
    });
});