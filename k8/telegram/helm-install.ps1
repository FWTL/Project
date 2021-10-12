param(
    [Parameter(Mandatory=$True)]
    [System.String]
    $AccountId,

    [Parameter(Mandatory=$True)]
    [System.String]
    $PathToChart
)

$installationName = "telegram-" + $AccountId
$pathToPatchFile = $PathToChart + "\\add-path-to-ingress.json";

helm install $installationName $PathToChart --namespace "telegram" --set accountId=$AccountId

kubectl rollout status deployment ("telegram-deployment-" + $AccountId)
kubectl patch ingress telegram-ingress --type json --patch "$(Get-Content $pathToPatchFile -Raw | % {$_.replace("{{id}}",$AccountId)})"