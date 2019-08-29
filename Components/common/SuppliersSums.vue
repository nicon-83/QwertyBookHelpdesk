<template>
    <md-card>
        <md-card-header class="suplr">
            <span class="md-title">Итог распределения потребности</span>
            <div class="right">
                <span class="">Позиций:</span>
                <span class="md-body-2 sum_list-value">{{this.allSupplSumsPos}}</span>
                <span class="sum_list-head">Сумма заказа:</span>
                <span class="md-body-2 sum_list-value">{{this.allSupplSumsSum}} руб.</span>
                <md-button class="md-icon-button" @click="showSumRaspr = !showSumRaspr">
                    <md-icon v-show="!showSumRaspr">keyboard_arrow_down</md-icon>
                    <md-icon v-show="showSumRaspr">keyboard_arrow_up</md-icon>
                    <md-tooltip md-direction="top">Подробнее</md-tooltip>
                </md-button>
            </div>
        </md-card-header>
        <md-table v-show="showSumRaspr" v-model="suppliersSums">
            <md-table-row v-show="showSumRaspr" slot="md-table-row" slot-scope="{ item }">
                <md-table-cell md-label="Прайс-лист" md-numeric>{{ item.PriceName }}</md-table-cell>
                <md-table-cell md-label="Дата">{{ item.PriceDate }}</md-table-cell>
                <md-table-cell md-label="Минимум">{{ item.OrderMinSum }}</md-table-cell>
                <md-table-cell md-label="Заказано">{{ item.Cnt1 }}</md-table-cell>
                <md-table-cell md-label="Сумма">{{ item.Sum1 }}</md-table-cell>
            </md-table-row>
        </md-table>
    </md-card>
</template>

<script>
  export default {
    name: "SuppliersSums",
    props: {
      suppliersSums:{
        default: []
      }
    },
    data: ()=>{
      return {
        showSumRaspr: false,
      }
    },
    computed: {
      allSupplSumsPos() {
        let pos = 0;
        if (Array.isArray(this.suppliersSums)) {
          this.suppliersSums.forEach((item) => {
            pos = pos + item.Cnt1
          });
        }
        return pos
      },
      allSupplSumsSum() {
        let sum = 0;
        if (Array.isArray(this.suppliersSums)) {
          this.suppliersSums.forEach((item) => {
            sum = sum + item.Sum1
          });
        }
        return sum
      },
    },
  }
</script>

<style scoped>
    .suplr {
        display: flex;
        flex-direction: row;
        justify-content: space-between;
    }

        .suplr.md-card-header:first-child > .md-title:first-child {
            margin-top: 0;
        }

        .suplr .md-card-header:last-child {
            margin-top: 0;
        }

        .suplr .md-title {
            line-height: 40px;
            margin-top: 0;
        }

        .suplr .right {
            line-height: 40px;
            display: inline-block;
        }
</style>
