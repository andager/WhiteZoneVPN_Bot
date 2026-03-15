using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using static Telegram.Bot.TelegramBotClient;
using Serilog;
class Program
{
    // Это клиент для работы с Telegram Bot API, который позволяет отправлять сообщения, управлять ботом, подписываться на обновления и многое другое.
    private static ITelegramBotClient _botClient;

    // Это объект с настройками работы бота. Здесь мы будем указывать, какие типы Update мы будем получать, Timeout бота и так далее.
    private static ReceiverOptions _receiverOptions;

    static async Task Main()
    {
        _botClient = new TelegramBotClient("8684764103:AAGNVjl3eOD6p6XgKyCzgY4VyGwrcqvHN-o"); // Присваиваем нашей переменной значение, в параметре передаем Token, полученный от BotFather

        _receiverOptions = new ReceiverOptions // Также присваем значение настройкам бота
        {
            AllowedUpdates = new[] // Тут указываем типы получаемых Update`ов, о них подробнее расказано тут https://core.telegram.org/bots/api#update
            {
                UpdateType.Message, // Сообщения (текст, фото/видео, голосовые/видео сообщения и т.д.)
                UpdateType.CallbackQuery
            },
            // Параметр, отвечающий за обработку сообщений, пришедших за то время, когда ваш бот был оффлайн
            // True - не обрабатывать, False (стоит по умолчанию) - обрабаывать
            //DropPendingUpdates = false
        };

        using var cts = new CancellationTokenSource();

        // UpdateHander - обработчик приходящих Update`ов
        // ErrorHandler - обработчик ошибок, связанных с Bot API
        _botClient.StartReceiving(UpdateHandler, ErrorHandler, _receiverOptions, cts.Token); // Запускаем бота

        Console.WriteLine("Бот запущен");

        await Task.Delay(-1); // Устанавливаем бесконечную задержку, чтобы наш бот работал постоянно
    }

    /// <summary>
    /// Метод для обработки всех изменений связаных с ботом
    /// </summary>
    /// <param name="botClient"> клиент для работы с Telegram Bot API </param>
    /// <param name="update"> все пришедшие изменения боту </param>
    private static async Task UpdateHandler(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        // try-catch, чтобы наш бот не "падал" в случае каких-либо ошибок
        try
        {

            // switch, чтобы обрабатывать приходящие Update
            switch (update.Type)
            {

                // case для обработки пришедших сообщений от пользователя
                case UpdateType.Message:
                    {
                        // эта переменная будет содержать в себе все связанное с сообщениями
                        var message = update.Message;

                        // From - это от кого пришло сообщение (или любой другой Update)
                        var user = message.From;

                        // Выводим на экран то, что пишут нашему боту, а также небольшую информацию об отправителе
                        Console.WriteLine($"{user.FirstName} ({user.Id}) написал сообщение: {message.Text}");

                        // Chat - содержит всю информацию о чате
                        var chat = message.Chat;

                        // Добавляем проверку на тип Message
                        switch (message.Type)
                        {

                            // Тут понятно, текстовый тип
                            case MessageType.Text:
                                {
                                    // команда при запуске бота, открывает главное меню
                                    if (message.Text == "/start")
                                    {
                                        // inline кнопки под сообщением
                                        var btnStart = new InlineKeyboardMarkup(new[]
                                        {
                                    new[]
                                    {
                                        InlineKeyboardButton.WithCallbackData("👤Мой профиль", "btnProfile"),
                                    },
                                    new[]
                                    {
                                        InlineKeyboardButton.WithCallbackData("❤️Пригласить друга", "btnReferal"),
                                    },
                                    new[]
                                    {
                                        InlineKeyboardButton.WithCallbackData("🆘Подержка", "btnHelp")
                                    }
                                });

                                        // отправка обучающего видео ролика и inline кнопок
                                        using (var videoLocal = File.OpenRead("D:\\Project VisualStudio\\WhiteZoneVPN_Bot\\WhiteZoneVPN_Bot\\Images\\Logo.png"))
                                        {
                                            await botClient.SendPhoto(
                                            chat.Id,
                                            photo: "https://habrastorage.org/r/w1560/getpro/habr/upload_files/235/06c/3a4/23506c3a4fffdf1436bb3a9f0f456690.png",
                                            caption: "В этом минутоном видео мы подробно показали процесс установки и настройки VPN",
                                            replyMarkup: btnStart
                                            );
                                        }

                                        return;
                                    }

                                    // команда для получения инструкции на сторонем сайте
                                    if (message.Text == "/instruktion")
                                    {
                                        // inline кнопки под сообщением
                                        var btnInstruktion = new InlineKeyboardMarkup(new[]
                                        {
                                    new[]
                                    {
                                        InlineKeyboardButton.WithUrl("❕Инструкция", "https://dzen.ru"),
                                    }
                                });

                                        // отпарвка сообещния пользователю с сылкой на инструкцию
                                        await botClient.SendMessage(
                                            chat.Id,
                                            "Инструкция для всех устройств доступна по кнопке ниже, если вдруг у Вас что-то не получается — то, напишите нам в поддержку и мы обязательно поможем!",
                                            replyMarkup: btnInstruktion
                                        );

                                        return;
                                    }

                                    // команда для покупки или продления подписки
                                    if (message.Text == "/buy")
                                    {
                                        return;
                                    }
                                }
                                return;

                            // обработка всех остальных сообщений от пользователя, не связанных с ботом
                            default:
                                {
                                    // отправка сообщения пользователю, что данной команды не существует
                                    await botClient.SendMessage(
                                        chat.Id,
                                        "Данной команды не существует"
                                    );
                                    return;
                                }
                        }
                    }

                // case для обработки нажатий пользователя на inline кнопки
                case UpdateType.CallbackQuery:
                    {

                        // Переменная, которая будет содержать в себе всю информацию о кнопке, которую нажали
                        var callbackQuery = update.CallbackQuery;

                        // Аналогично и с Message мы можем получить информацию о чате, о пользователе и т.д.
                        var user = callbackQuery.From;

                        // Выводим на экран нажатие кнопки
                        Console.WriteLine($"{user.FirstName} ({user.Id}) нажал на кнопку: {callbackQuery.Data}");

                        // Вот тут нужно уже быть немножко внимательным и не путаться!
                        // Мы пишем не callbackQuery.Chat , а callbackQuery.Message.Chat , так как
                        // кнопка привязана к сообщению, то мы берем информацию от сообщения.
                        var chat = callbackQuery.Message.Chat;

                        // Добавляем блок switch для проверки кнопок
                        // Data - это придуманный нами id кнопки, мы его указывали в параметре
                        // callbackData при создании кнопок. У меня это button1, button2 и button3
                        switch (callbackQuery.Data)
                        {

                            // нажатие на кнопку "👤Мой профиль"
                            case "btnProfile":
                                {

                                    // В этом типе клавиатуры обязательно нужно использовать следующий метод
                                    // Для того, чтобы отправить телеграмму запрос, что мы нажали на кнопку
                                    await botClient.AnswerCallbackQuery(callbackQuery.Id);

                                    // создание inline кнопки для подключения
                                    var btnAdd = new InlineKeyboardMarkup(new[]
                                    {
                                        new[]
                                        {
                                            InlineKeyboardButton.WithCallbackData("🚀 Подключиться", "btnConect"),
                                        }
                                    });

                                    // отправка сообщения пользователю о его состоянии акаунта
                                    await botClient.SendMessage(
                                        chat.Id, $"👤 Ваш ID: {user.Id}\n📅 Дней доступно {0}\n📲 Устройств подключено {0}",
                                        replyMarkup: btnAdd
                                        );
                                    return;
                                }

                            // нажатие на кнопку "❤️Пригласить друга"
                            case "btnReferal":
                                {
                                    // В этом типе клавиатуры обязательно нужно использовать следующий метод
                                    // Для того, чтобы отправить телеграмму запрос, что мы нажали на кнопку
                                    await botClient.AnswerCallbackQuery(callbackQuery.Id);

                                    await botClient.SendMessage(chat.Id, "Спасио что пользуетесь WhiteZone VPN, мы вам очень благодарны. Приглашайте свойих друзейЮ нас буудет становиться больше и мы будем быстрее развиваться. Вот ссылка на наш VPN https://t.me/WhiteZoneVPN_bot");

                                    return;
                                }
                            // нажатие на кнопку "🆘Подержка"
                            case "btnHelp":
                                {
                                    // В этом типе клавиатуры обязательно нужно использовать следующий метод
                                    // Для того, чтобы отправить телеграмму запрос, что мы нажали на кнопку
                                    await botClient.AnswerCallbackQuery(callbackQuery.Id);

                                    await botClient.SendMessage(chat.Id, "Жопа 3");

                                    return;
                                }
                            // нажатие на кнопку "❕Инструкция"
                            case "btnInstruktion":
                                {
                                    // В этом типе клавиатуры обязательно нужно использовать следующий метод
                                    // Для того, чтобы отправить телеграмму запрос, что мы нажали на кнопку
                                    await botClient.AnswerCallbackQuery(callbackQuery.Id);

                                    await botClient.SendMessage(chat.Id, "Жопа 4");

                                    return;
                                }
                            // нажатие на кнопку "🚀 Подключиться"
                            case "btnConect":
                                {
                                    // В этом типе клавиатуры обязательно нужно использовать следующий метод
                                    // Для того, чтобы отправить телеграмму запрос, что мы нажали на кнопку
                                    await botClient.AnswerCallbackQuery(callbackQuery.Id);

                                    // создание кнопок с выбором модели телефона
                                    var btnPhone = new InlineKeyboardMarkup(new[]
                                    {
                                        new[]
                                        {
                                            InlineKeyboardButton.WithCallbackData("🍏 IOS", "btnIos")
                                        },
                                        new[]
                                        {
                                            InlineKeyboardButton.WithCallbackData("📱 Android", "btnAndroid"),
                                            InlineKeyboardButton.WithCallbackData("📱 Huawei", "btnHuawei")
                                        }
                                    });

                                    // отпарвка сообщения пользователю с выбором устройства и инструкцией
                                    await botClient.SendMessage(
                                        chat.Id,
                                        "Выберите устройство и мгновенно получите доступ к VPS. Подключение занимает меньше минуты — никаких сложных настроек. Выберите устройство и следуйте инструкции.",
                                        replyMarkup: btnPhone
                                        );

                                    return;
                                }

                        }
                    }
                    return;
            }
        }

        // Обработка возникших и не обработаных исключений
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }

    private static Task ErrorHandler(ITelegramBotClient botClient, Exception error, CancellationToken cancellationToken)
    {
        // Тут создадим переменную, в которую поместим код ошибки и её сообщение 
        var ErrorMessage = error switch
        {
            ApiRequestException apiRequestException
                => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => error.ToString()
        };

        Console.WriteLine(ErrorMessage);
        return Task.CompletedTask;
    }

}