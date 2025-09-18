var ssgApp = angular.module('ssg.orgChartModule', ["kendo.directives"]);
ssgApp.controller('orgChartController', function ($rootScope, $scope, $location, appSetting, settingService, dashboardService, $timeout, $q) {
    // create a message to display in our view
    var defaultColor = "#1696d3";
    $scope.title = 'Organization Chart';
    $scope.model = {
        departmentId: null, maxLvl: 3
    }
    var allDepartments = [];
    if (sessionStorage.getItemWithSafe("departments")) {
        allDepartments = JSON.parse(sessionStorage.getItemWithSafe("departments"));
    }
    $scope.departmentOptions = {
        dataTextField: "name",
        dataValueField: "id",
        template: showCustomDepartmentTitle,
        valuePrimitive: false,
        checkboxes: false,
        autoBind: true, loadOnDemand: true,
        filter: "contains",
        filtering: async function (option) {
            await getDepartmentByFilter(option);
        },
        select: function (e) {
            if(e.sender.value()){
                e.sender.options.autoClose = true;
            }
        },
        change: async function (e) {
            if (!e.sender.value()) {
                await setDataDepartment(allDepartments);
            }
        },
        // valueTemplate: (e) => showCustomField(e, ['name', 'userCheckedHeadCount'])
        valueTemplate: (e) => showCustomField(e, ['name'])
    };
    async function setDataDepartment(dataDepartment, oldDepartmentId) {
        if(oldDepartmentId){
            var departmentDetail = await settingService.getInstance().departments.getDetailDepartment({
                id: oldDepartmentId
            }).$promise;
            if(departmentDetail.isSuccess){
                let valueSearchDepartment = departmentDetail.object.object;
                dataDepartment = dataDepartment.concat(valueSearchDepartment);
            }
        }

        var dataSource = new kendo.data.HierarchicalDataSource({
            data: dataDepartment,
            schema: {
                model: {
                    children: "items"
                }
            }
        });
        var departmentTree = $("#departmentId").data("kendoDropDownTree");
        if (departmentTree) {
            departmentTree.setDataSource(dataSource);
            if(departmentTree._value){
                departmentTree.options.autoClose = true;
            }
            if(oldDepartmentId){
                departmentTree.options.autoClose = false;
                departmentTree.value(oldDepartmentId);
            }
        }
        // if (departmentTree) {
        //     departmentTree.setDataSource(dataSource);
        // }
    }
    async function getDepartmentByFilter(option) {
        let departmentId = option.sender.value();
        if (!option.filter) {
            option.preventDefault();
        } else {
            let filter = option.filter && option.filter.value ? option.filter.value : "";
            arg = {
                predicate: "(name.contains(@0) or code.contains(@1)) or UserDepartmentMappings.Any(User.FullName.contains(@2))",
                predicateParameters: [filter.trim(), filter.trim(), filter.trim()],
                page: 1,
                limit: appSetting.pageSizeDefault,
                order: ""
            }
            if (filter) {
                res = await settingService.getInstance().departments.getDepartmentByFilter(arg).$promise;
                if (res.isSuccess) {
                    await setDataDepartment(res.object.data, departmentId);
                }
            } else {
                await setDataDepartment(allDepartments);
            }


        }
    }
    // function showCustomDepartmentTitle(e) {
    //     let model = e.item;
    //     if (model.userCheckedHeadCount) {
    //         return `${model.code} - ${model.name} - ${model.userCheckedHeadCount}`
    //     } else {
    //         return `${model.code} - ${model.name}`;
    //     }
    // }
    function showCustomField(model, fields) {
        var items = [];
        fields.forEach(x => {
            if (model[x]) {
                items.push(model[x]);
            }
        });
        return items.join(' - ');
    }

    $scope.search = async function () {
        var res = await dashboardService.getInstance().dashboard.getEmployeeNodesByDepartment({ departmentId: $scope.model.departmentId, maxLvl: $scope.model.maxLvl }).$promise;
        if (res.isSuccess) {
            $scope.orgChartDiagram.setDataSource(new kendo.data.HierarchicalDataSource({
                data: res.object,
                schema: {
                    model: {
                        children: "items",
                        hasChildren: "hasChild"
                    }
                }
            }));
            $scope.orgChartDiagram.zoom(1, 1);
            $scope.orgChartDiagram.bringIntoView($scope.orgChartDiagram.boundingBox());
        }
    }
    function visualTemplate(options) {
        var dataviz = kendo.dataviz;
        var g = new dataviz.diagram.Group();
        var dataItem = options.dataItem;

        g.append(new dataviz.diagram.Rectangle({
            width: 370,
            height: 85,
            stroke: {
                width: 0
            },
            fill: {
                gradient: {
                    type: "linear",
                    stops: [{
                        color: dataItem.colorCode ? dataItem.colorCode : defaultColor,
                        offset: 0,
                        opacity: 0.5
                    }, {
                        color: dataItem.colorCode ? dataItem.colorCode : defaultColor,
                        offset: 1,
                        opacity: 1
                    }]
                }
            }
        }));


        g.append(new dataviz.diagram.TextBlock({
            text: dataItem.departmentName,
            x: 90,
            y: 20,
            fill: "#000000",
            fontSize: 10,
            fontWeight: "bold"
        }));
        g.append(new dataviz.diagram.TextBlock({
            text: dataItem.departmentCaption,
            x: 90,
            y: 40,
            fill: "#000000",
            fontSize: 9
        }));

        g.append(new dataviz.diagram.TextBlock({
            text: dataItem.employeeName ? dataItem.employeeName : "<Empty>",
            x: 90,
            y: 60,
            fill: "#000000",
            fontSize: 9
        }));
        g.append(new dataviz.diagram.Image({
            source: dataItem.employeeImage ? baseUrlApi.replace('/api','')+dataItem.employeeImage : "ClientApp/assets/images/avatar.png",
            x: 3,
            y: 3,
            width: 78,
            height: 78
        }));

        return g;
    }
    function ngOnInit() {
        $timeout(async function () {
            await setDataDepartment(allDepartments);
        }, 0)

    }
    ngOnInit();

    $scope.orgChartOptions = {
        zoom: 0.8,
        dataSource: new kendo.data.HierarchicalDataSource({
            data: [],
            schema: {
                model: {
                    children: "items",
                    hasChildren: "hasChild"
                }
            }
        }),
        layout: {
            type: "tree",
            subtype: "down",
            horizontalSeparation: 120,
            verticalSeparation: 120
        },
        shapeDefaults: {
            visual: visualTemplate
        },
        connectionDefaults: {
            stroke: {
                color: "#979797",
                width: 2
            },
            fromConnector: "bottom",
            toConnector: "top"
        }
    }

    $scope.export = function () {
        $scope.orgChartDiagram.exportPDF({ paperSize: "auto", margin: { left: "1cm", top: "1cm", right: "1cm", bottom: "1cm" } }).done(function (data) {
            kendo.saveAs({
                dataURI: data,
                fileName: "aeon-orgchart.pdf"
            });
        });
    }

});