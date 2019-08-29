<template>
  <router-view></router-view>
</template>

<script>


  export default {
    name: 'app',
    mounted: function() {
      this.$material.locale =  {
        startYear: 1900,
        endYear: 2099,
        dateFormat: 'YYYY-MM-DD',
        days: ['Воскресенье', 'Понедельник', 'Вторник', 'Среда', 'Четверг', 'Пятница', 'Суббота'],
        shortDays: ['Вс', 'Пн', 'Вт', 'Ср', 'Чт', 'Пт', 'Сб'],
        shorterDays: ['В', 'П', 'В', 'С', 'Ч', 'П', 'С'],
        months: ['Январь', 'Февраль', 'Март', 'Апрель', 'Май', 'Июнь', 'Июль', 'Август', 'Сентябрь', 'Октябрь', 'Ноябрь', 'Декабрь'],
        shortMonths: ['Янв', 'Фев', 'Март', 'Апр', 'Май', 'Июнь', 'Июль', 'Авг', 'Сен', 'Окт', 'Ноя', 'Дек'],
        shorterMonths: ['Я', 'Ф', 'М', 'А', 'М', 'Июнь', 'Июль', 'А', 'С', 'О', 'Н', 'Д'],
        firstDayOfAWeek: 1
      }
    }
	}

</script>
