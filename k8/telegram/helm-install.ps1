param(
    [Parameter(Mandatory=$True)]
    [System.String]
    $AccountId,

    [Parameter(Mandatory=$True)]
    [System.String]
    $PathToChart
)

$installationName = "Telegram-" + $AccountId
helm install $installationName $PathToChart --namespace "Telegram" --set accountId=$AccountId
kubectl patch ingress telegram-ingress --type json --patch "$(Get-Content add-path-to-ingress.json -Raw | % {$_.replace("{{id}}",$AccountId)})"