// ========== Init data ==========
window.getEventsData = function () {
  return [
    {
      title: "Budget Control metting",
      start: "2024-07-22T09:00:00",
      end: "2024-07-22T10:00:00",
      extendedProps: {
        eventTime: true,
        eventGuest: "Other",
        eventGuestNO: 15,
      },
      background: "var(--color-gradient-cyan-se)",
    },
    {
      title: "Best practice season 1 2024",
      start: "2024-07-22T12:00:00",
      end: "2024-07-22T14:00:00",
      extendedProps: {
        eventContent: "Owner Dept: <span class='font-medium'>HR</span>",
        eventSubContent:
          "Organizer: <span class='font-medium text-white'>vanthanh.nguyen</span>",
      },
      background: "var(--color-gradient-purple-se)",
    },
    {
      title: "Cleaning day",
      start: "2024-07-23T10:00:00",
      end: "2024-07-23T12:00:00",
      extendedProps: {
        eventContent: "Owner Dept: <span class='font-medium'>CSR</span>",
        eventSubContent:
          "Organizer: <span class='font-medium text-white'>thivan.nguyen</span>",
      },
      background: "var(--color-gradient-blue-se)",
    },
    {
      title: "NVL Store Opening",
      start: "2024-07-23T12:00:00",
      end: "2024-07-23T12:30:00",
      extendedProps: {
        eventContent: "Owner Dept: <span class='font-medium'>NVL store</span>",
        eventSubContent:
          "Organizer: <span class='font-medium text-white'>hiep.nguyen</span>",
      },
      background: "var(--color-gradient-cyan-se)",
    },
    {
      title: "Hue GMS Combat meeting",
      start: "2024-07-24T09:00:00",
      end: "2024-07-24T10:00:00",
      extendedProps: {
        eventContent: "Owner Dept: <span class='font-medium'>Cons</span>",
        eventSubContent:
          "Organizer: <span class='font-medium text-white'>khoa.nguyen</span>",
      },
      background: "var(--color-gradient-purple-se)",
    },
    {
      title: "Monthly meeting",
      start: "2024-07-24T10:00:00",
      end: "2024-07-24T10:30:00",
      extendedProps: {
        eventContent: "Owner Dept: <span class='font-medium'>Academy</span>",
        eventSubContent:
          "Organizer: <span class='font-medium text-white'>du.hua</span>",
      },
      background: "var(--color-gradient-cyan-se)",
    },
    {
      title: "Policy meeting",
      start: "2024-07-25T12:00:00",
      end: "2024-07-25T14:00:00",
      extendedProps: {
        eventContent: "Owner Dept: <span class='font-medium'>BOED</span>",
        eventSubContent:
          "Organizer: <span class='font-medium text-white'>thao.vu</span>",
      },
      background: "var(--color-gradient-blue-se)",
    },
  ];
};

// ========== Create calendar ==========
window.createCalendar = function (paramView, paramEventsData) {
  const initialView = paramView || "timeGridWeek";
  const eventsData = paramEventsData || getEventsData();
  const calendarEl = document.getElementById("calendar");

  if (!calendarEl) {
    return;
  }

  // plugins: ["interaction", "dayGrid", "timeGrid"],
  let calendar = new FullCalendar.Calendar(calendarEl, {
    initialView: initialView,
    events: eventsData,
    editable: true,
    selectable: true,
    eventTimeFormat: {
      hour: "2-digit",
      minute: "2-digit",
      meridiem: true,
    },
    headerToolbar: {
      start: "timeGridDay,timeGridWeek,dayGridMonth",
    },
    customButtons: {
      btnAddEvent: {
        text: "Add Event",
        click: function () {
          toggleModal("bst-modal-event", "bst-modal-event-shadow");
        },
      },
    },
    eventContent: function (arg) {
      const startH = calendar.formatDate(arg.event.start, {
        hour: "numeric",
        minute: "2-digit",
        meridiem: "long",
      });
      const endH = calendar.formatDate(arg.event.end, {
        hour: "numeric",
        minute: "2-digit",
        meridiem: "long",
      });
      const hours = `${startH} - ${endH}`;
      const props = arg.event.extendedProps;

      const content = props.eventTime
        ? `<i class="bi bi-clock"></i> ${hours}`
        : props.eventContent;

      const subContent =
        props.eventGuest && props.eventGuestNO
          ? `+ ${props.eventGuest} ${props.eventGuestNO}`
          : props.eventSubContent;

      return {
        html: `
            <div class="event-card">
                <p class="card-title">${arg.event.title}</p>
                <p class="card-content">${content}</p>
                <p class="card-sub-content">${subContent}</p>
            </div>
            `,
      };
    },
    hiddenDays: [0, 5, 6],
    slotMinTime: "09:00",
    slotMaxTime: "15:00",
    expandRows: true,
    allDaySlot: false,
    dayHeaderContent: function (args) {
      const date = args.date;
      const weekday = date.toLocaleDateString("en-US", {
        weekday: "long",
      });
      const day = date.toLocaleDateString("en-US", {
        day: "numeric",
      });
      const fullDate = `<p class="text-sm"><b>${day}</b> <br> <span class="text-gray-light font-normal">${weekday}</span></p>`;

      return { html: fullDate };
    },
    aspectRatio: 1,
    // set gradient to calendar
    eventDidMount: function (info) {
      if (info.event.extendedProps.background) {
        info.el.style.background = info.event.extendedProps.background;
      }
    },
  });

  calendar.render();

  // hidden header toolbar buttons
  const views = ["timeGridDay", "timeGridWeek", "dayGridMonth"];
  views.forEach(function (view) {
    let button = document.querySelector(`#calendar .fc-${view}-button`);
    if (button) {
      button.style.display = "none";
    }
  });
  // hidden header toolbar buttons: today, prev, next
  let buttonToday = document.querySelector(`#calendar .fc-today-button`);
  let buttonPrev = document.querySelector(`#calendar .fc-prev-button`);
  let buttonNext = document.querySelector(`#calendar .fc-next-button`);
  // buttonToday.style.display = "none";
  // buttonPrev.style.display = "none";
  // buttonNext.style.display = "none";

  // count events
  let boxNOEvents = document.querySelector(
    "#bst-finance-schedule .bst-no-events"
  );
  boxNOEvents.innerHTML = eventsData.length;

  // add UTC+7 to axis
  let axis = document.querySelector("#calendar .fc-timegrid-axis-frame");
  axis.innerHTML = "UTC+7";
};

window.switchViewCalendar = function (view, calendarButton) {
  // switch view
  let button = document.querySelector(`#calendar .fc-${view}-button`);
  if (button) {
    button.click();
  }
  // active button
  let calendarButtons = document.querySelectorAll(
    "#bst-finance-schedule .btn-schedule-datetime"
  );
  calendarButtons.forEach(function (btn) {
    btn.classList.remove("active");
  });
  calendarButton.classList.add("active");
};

// ========== Create calendar mini ==========
window.createCalendarMini = function () {
  const today = new Date();
  let currentMonth = today.getMonth();
  let currentYear = today.getFullYear();

  const months = [
    "January",
    "February",
    "March",
    "April",
    "May",
    "June",
    "July",
    "August",
    "September",
    "October",
    "November",
    "December",
  ];

  const boxMonth = document.getElementById("month");
  const boxYear = document.getElementById("year");
  const prevMonth = document.getElementById("prevMonth");
  const nextMonth = document.getElementById("nextMonth");
  const boxMonthCalendar = document.querySelector(
    "#bst-finance-schedule .bst-month"
  );
  const boxYearCalendar = document.querySelector(
    "#bst-finance-schedule .bst-year"
  );

  showCalendar(currentMonth, currentYear);

  if (prevMonth) {
    prevMonth.addEventListener("click", () => {
      currentMonth--;
      if (currentMonth < 0) {
        currentMonth = 11;
        currentYear--;
      }
      showCalendar(currentMonth, currentYear);
    });
  }

  if (nextMonth) {
    nextMonth.addEventListener("click", () => {
      currentMonth++;
      if (currentMonth > 11) {
        currentMonth = 0;
        currentYear++;
      }
      showCalendar(currentMonth, currentYear);
    });
  }

  function extractDateTime(dateString) {
    const date = new Date(dateString);
    return {
      year: date.getFullYear(),
      month: months[date.getMonth()],
      date: date.getDate(),
    };
  }

  function showCalendar(month, year) {
    const firstDay = new Date(year, month, 1);
    const lastDay = new Date(year, month + 1, 0);

    if (!boxMonth || !boxYear || !boxMonthCalendar || !boxYearCalendar) {
      return;
    }

    boxMonth.innerHTML = months[month];
    boxYear.innerHTML = year;
    boxMonthCalendar.innerHTML = months[month];
    boxYearCalendar.innerHTML = year;

    const eventsData = getEventsData();
    let selectedDays = [];
    let date = 1;
    let html = "";
    for (let i = 0; i < 6; i++) {
      html += "<tr class='text-center'>";
      for (let j = 0; j < 7; j++) {
        eventsData.forEach(function (eventData) {
          let eventStartDay = eventData.start;
          let extractedDateTime = extractDateTime(eventStartDay);
          if (
            extractedDateTime.year === year &&
            extractedDateTime.month === months[month] &&
            extractedDateTime.date === date
          ) {
            selectedDays.push(date);
          }
        });
        if (i === 0 && j < firstDay.getDay()) {
          html += "<td></td>";
        } else if (date > lastDay.getDate()) {
          break;
        } else {
          const className = selectedDays.includes(date) ? "selected-date" : "";
          html += `<td class="p-1"><span class='p-1 ${className}'>${date}</span></td>`;
          date++;
        }
      }
      html += "</tr>";
    }
    document.querySelector("tbody").innerHTML = html;
  }
};
