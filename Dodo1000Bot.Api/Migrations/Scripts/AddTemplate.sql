INSERT INTO notification_templates
(
    `NotificationType`,
    `MessengerType`,
    `LanguageCode`,
    `Template`
)
VALUES
    (
        $(NotificationType),
        $(MessengerType),
        $(LanguageCode),
        $(Template)
    );