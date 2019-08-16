var InventoryList = {
    template: '#list',
    props: ['invdept', 'inventories'],
    data: function () {
        return {};
    },
    methods: {
        get_uncheck: function (deptID) {
            this.$emit('getunchecklist', deptID)
        }
    }
};

var InventoryCamera = {
    template: '#camera',
    props: ['scanner', 'active_camera_id', 'cameras', 'scans', 'inventories'],
    data: function () {
        return {};
    },
    mounted: function () {
        inv.phonecamera();
    },
    methods: {
        back: function (type) {
            this.$emit('list', type)
        },
        formatName: function (name) {
            return name || '(unknown)';
        },
        selectCamera: function (camera) {
            this.active_camera_id = camera.id;
            this.scanner.start(camera);
        }
    }
};

var inv = new Vue({
    el: '#Inventory',
    data: {
        scanner: null,
        active_camera_id: null,
        cameras: [],
        scans: [],
        invdept: [],
        inventories: [],
        type: 1
    },
    components: {
        'inventorylist': InventoryList,
        'inventorycamera': InventoryCamera
    },
    mounted: function () {
        var inv = this;
        $.ajax({
            type: "POST",
            url: "InventoryCheck.asmx/GetUnCheckInventoryGroupByDept",
            contentType: "application/json",
            dataType: "json",
            success: function (response, textStatus) {
                if (textStatus == "success") {
                    var invs = response.d;
                    inv.invdept = [];

                    invs.forEach(function (item, index, array) {
                        inv.invdept.push({
                            'DeptID': item.DeptID,
                            'DeptName': item.DeptName,
                            'Amount': item.Amount
                        });
                    });
                }
            },
            error: function (response) {
                console.log(response);
            },
            complete: function (response) {

            }
        });
    },
    methods: {
        getunchecklist: function (deptID) {
            var inv = this;
            $.ajax({
                type: "POST",
                url: "InventoryCheck.asmx/GetUnCheckInventory",
                data: '{"deptId":"' + deptID + '"}',
                contentType: "application/json",
                dataType: "json",
                success: function (response, textStatus) {
                    if (textStatus === 'success') {
                        var invs = response.d;
                        inv.inventories = [];

                        invs.forEach(function (item, index, array) {
                            inv.inventories.push({
                                'HOSP_ID': item.HOSP_ID,
                                'OWNER': item.OWNER,
                                'ASS_NO': item.ASS_NO,
                                'ASS_SERNO': item.ASS_SERNO,
                                'QR_CODE': item.QR_CODE,
                                'CURKEEP_DEPT': item.CURKEEP_DEPT,
                                'CURKEEP_DEPT_NAME': item.CURKEEP_DEPT_NAME,
                                'CURKEEPER': item.CURKEEPER,
                                'CURPLACE': item.CURPLACE,
                                'BUY_DATE': item.BUY_DATE,
                                'ASS_PNAME': item.ASS_PNAME,
                                'SYSTEM_MARK': 'N'
                            });
                        });
                        inv.type = 0;
                    }
                },
                error: function (response) {
                    console.log(response);
                },
                complete: function () {

                }
            })
        },

        list: function (type) {
            inv.type = type;
        },

        phonecamera: function () {
            var self = this;
            self.scanner = new Instascan.Scanner({ video: document.getElementById('preview'), mirror: false, scanPeriod: 1 });
            self.scanner.addListener('scan', function (content, image) {
                self.scans.unshift({ date: +(Date.now()), content: content });

                var find = inv.inventories.find(function (item, index, array) {
                    return item.QR_CODE === content;
                });

                var ans = inv.inventories.some(function (item, index, array) {
                    return item.QR_CODE === content;
                });

                if (ans) {
                    find.SYSTEM_MARK = 'Y';
                }
            });
            Instascan.Camera.getCameras().then(function (cameras) {
                self.cameras = cameras;
                if (cameras.length > 0) {
                    self.active_camera_id = cameras[1].id;
                    self.scanner.start(cameras[1]);
                } else {
                    console.error('No cameras found.');
                }
            }).catch(function (e) {
                console.error(e);
            });
        }
    }
});