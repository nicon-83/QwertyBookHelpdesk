export const gridMixin = {
    props: {
        data: Array,
        columns: Array,
        filterKey: String,
        filterColumn: String,
        searchKey: String
    },
    data: function () {
        var sortOrders = {}
        this.columns.forEach(function (key) {
            sortOrders[key] = 1
        })
        return {
            sortKey: '',
            sortOrders: sortOrders
        }
    },
    computed: {
        filteredData: function () {
            var sortKey = this.sortKey
            var filterKey = this.filterKey && this.filterKey.toLowerCase()
            var filterColumn = this.filterColumn
            var searchKey = this.searchKey && this.searchKey.toLowerCase()
            var order = this.sortOrders[sortKey] || 1
            var data = this.data
            if (searchKey) {
                data = data.filter(function (item, i) {
                    return Object.keys(item).some(function (key) {
                        return String(item[key]).toLowerCase().indexOf(searchKey) > -1
                    })
                })
            }
            if (filterKey && filterColumn && 1) {
                data = data.filter(function (item, i) {
                    return item[filterColumn].toLowerCase() == filterKey
                })
            }
            if (sortKey) {
                data = data.slice().sort(function (a, b) {
                    a = a[sortKey]
                    b = b[sortKey]
                    return (a === b ? 0 : a > b ? 1 : -1) * order
                })
            }
            return data
        }
    },
    filters: {
        capitalize: function (str) {
            return str.charAt(0).toUpperCase() + str.slice(1)
        }
    },
    methods: {
        sortBy: function (key) {
            this.sortKey = key
            this.sortOrders[key] = this.sortOrders[key] * -1
        }
    }
}
