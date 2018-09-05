var vm = new Vue({
    el: '#contactSearch',
    components: {
    },
    data: {
        sortKey: '',
        sortOrders: [],
        searchResults: [],
        filterKey: '',
        columns: ['firstName', 'lastName', 'email', 'phone1'],
        startRow: 0,
        rowsPerPage: 10,
        selectedContact: null,
    },
    methods: {
        sortBy: function (key) {
            this.sortKey = key
            this.sortOrders[key] = this.sortOrders[key] * -1;
        },
        getData: function () {
            var vm = this;
            $.ajax({
                type: "GET",
                traditional: true,
                async: false,
                cache: false,
                url: 'api/contact',
                context: document.body,
                success: function (results) {
                    console.log("47");
                    console.log(results);
                    vm.$nextTick(function () {
                        vm.searchResults = results
                    })

                },
                error: function (xhr) {
                    //debugger;  
                    console.log(xhr.responseText);
                    alert("Error has occurred..");
                }
            });
        },
        movePages: function (amount) {
            var newStartRow = this.startRow + (amount * this.rowsPerPage);
            if (newStartRow >= 0 && newStartRow < this.filteredData.length) {
                this.startRow = newStartRow;
            }
        },
        selectRow: function (data) {
            if (this.selectedContact === data) {
                this.selectedContact = null;
            } else {
                this.selectedContact = data;
            }
            
        }
        
    },
    watch: {
        filterKey: function () {
            this.startRow = 0;
            this.selectedContact = null;
        }
    },
    computed: {       
        filteredData: function () {
            var sortKey = this.sortKey
            var filterKey = this.filterKey && this.filterKey.toLowerCase()
            var order = this.sortOrders[sortKey] || 1
            var data = this.searchResults;
            if (filterKey) {
                data = data.filter(function (row) {
                    return Object.keys(row).some(function (key) {
                        return String(row[key]).toLowerCase().indexOf(filterKey) > -1
                    })
                })
            }
            if (sortKey) {
                data = data.slice().sort(function (a, b) {
                    a = a[sortKey]
                    b = b[sortKey]
                    return (a === b ? 0 : a > b ? 1 : -1) * order
                })
            }
            return data;
        },
        dataAfterPaging: function () {            
            this.filteredData
            var start = this.startRow * this.rowsPerPage;
            return this.filteredData.slice(this.startRow, this.startRow + this.rowsPerPage)
        }
    },
    created: function () {
        var sortOrders = {};
        for (var i = 0; i < this.columns.length; i++) {
            var key = this.columns[i];
            this.sortOrders[key] = 1
        }
       

    },
    mounted: function () {
        this.getData();
    },
    filters: {
        spacewords: function (str) {
            var strWordsHeader = '';
            switch (str.toLowerCase()) {
                case 'firstname':
                    strWordsHeader = 'First Name';
                    break;
                case 'lastname':
                    strWordsHeader = 'Last Name';
                    break;
                default:
                    strWordsHeader = str.charAt(0).toUpperCase() + str.slice(1)

            }
            return strWordsHeader;
        }
    },

})

