
// register the grid component
Vue.component('grid-layout', {
    props: {
        datagird: Array,
        columns: Array,
        rowsPerPage: Number,
        startRow: Number

    }
    ,template: "<div> <table>"+
        "< thead >"+
        "<tr>"+
            '<th v-for="key in columns">'+
                "{{ key | spacewords }}"+
            "</th>"+
        "</tr>"+
        "</thead>"+
        "<tbody>"+
            '<tr v-for="entry in datagird">'+
                '<td v-for="key in columns">'+
                    "{{ entry[key] }}" +
                "</td>"+
            "</tr>"+
        "</tbody>"+
    "</table>"+
        '<div id="page-navigation">'+
            '<button v-click=movePages(-1)>Back</button>'+
        //'<p>{{ startRow / rowsPerPage + 1}} out of {{this.datagird.length / rowsPerPage }}</p>'+
        '<button v-click=movePages(1) > Next</button >'+
        '</div >' +
    '</div >'
    , methods: {
        movePages: function (amount) {
            debugger;
            var total = 0;
            if (datagird && datagird.length) {
                total = datagird.length
            }
            var newStartRow = this.startRow + (amount * this.rowsPerPage);
            if (newStartRow >= 0 && newStartRow < total) {
                this.startRow = newStartRow;
            }
        }
    },
    filters: {
        orderByBusinessRules: function (data) {
            debugger;
            return data.slice().sort(function (a, b) {
                return a.power - b.power;
            });
        }
    }
});
